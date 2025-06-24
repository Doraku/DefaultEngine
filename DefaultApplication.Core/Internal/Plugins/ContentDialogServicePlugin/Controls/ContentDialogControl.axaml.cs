using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DefaultApplication.Controls.Behaviors;
using DefaultApplication.DependencyInjection;
using DefaultApplication.Services;

namespace DefaultApplication.Internal.Plugins.ContentDialogServicePlugin.Controls;

internal sealed partial class ContentDialogControl : Grid, IContentDialogService
{
    private readonly Task<TopLevel> _mainTopLevel;
    private readonly Stack<(object, TaskCompletionSource<IContentDialogService.DialogResult>)> _operations;

    private TaskCompletionSource<bool> _available;
    private TaskCompletionSource<IContentDialogService.DialogResult>? _contentDialogResult;
    private BindingExpressionBase? _marginBinding;

    public ContentDialogControl(IDelayed<TopLevel> mainTopLevel)
    {
        _mainTopLevel = mainTopLevel.Task.ContinueWith(
            async task =>
            {
                TopLevel topLevel = await task.ConfigureAwait(true);

                InstallOnTopLevel(topLevel);

                return topLevel;
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default).Unwrap();
        _operations = [];

        _available = new TaskCompletionSource<bool>();
        _available.SetResult(true);

        InitializeComponent();
    }

    private void InstallOnTopLevel(TopLevel topLevel)
    {
        void OnTopLevelTemplateApplied(object? sender, TemplateAppliedEventArgs e)
        {
            if (Parent is AdornerLayer adornerLayer)
            {
                adornerLayer.Children.Remove(this);
                AdornerLayer.SetAdornedElement(this, null);
            }

            TopLevel topLevel = (TopLevel)sender!;
            topLevel.TemplateApplied -= OnTopLevelTemplateApplied;
            _marginBinding?.Dispose();

            InstallOnTopLevel(topLevel);
        }

        topLevel.TemplateApplied += OnTopLevelTemplateApplied;

        if (topLevel.FindDescendantOfType<VisualLayerManager>()?.AdornerLayer is AdornerLayer adorner)
        {
            adorner.Children.Add(this);
            AdornerLayer.SetAdornedElement(this, adorner);

            if (topLevel is Window window)
            {
                _marginBinding = Bind(MarginProperty, new Binding("OffScreenMargin", BindingMode.OneWay) { Source = window });
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty
            && !change.GetNewValue<bool>())
        {
            Tag = null;
            _available.TrySetResult(true);
        }
    }

    public void OnContentDialogReturn(IContentDialogService.DialogResult result) => _contentDialogResult?.TrySetResult(result);

    public async Task<IContentDialogService.DialogResult> ShowAsync(object content, CancellationToken cancellationToken)
    {
        void OnContentDialogKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key is Key.Escape
                && ContentDialog.GetNoneContent(ContentDialogPresenter.Child) is { })
            {
                _contentDialogResult?.TrySetResult(IContentDialogService.DialogResult.None);
            }
            else if (e.Key is Key.Enter
                && ContentDialog.GetPrimaryContent(ContentDialogPresenter.Child) is { }
                && ContentDialog.GetCanReturnPrimary(ContentDialogPresenter.Child))
            {
                _contentDialogResult?.TrySetResult(IContentDialogService.DialogResult.Primary);
            }
        }

        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => ShowAsync(content, cancellationToken)).ConfigureAwait(false);
        }

        TopLevel topLevel = await _mainTopLevel.ConfigureAwait(true);

        if (Tag is { } && _contentDialogResult is { })
        {
            _operations.Push((Tag, _contentDialogResult));
            Opacity = 0;
            topLevel.KeyDown -= OnContentDialogKeyDown;
        }

        TaskCompletionSource<IContentDialogService.DialogResult> resultSource = new();
        IContentDialogService.DialogResult result = IContentDialogService.DialogResult.None;

        _operations.Push((content, resultSource));

        using (cancellationToken.Register(() => resultSource.TrySetResult(IContentDialogService.DialogResult.None)))
        {
            while (_operations.TryPop(out (object Content, TaskCompletionSource<IContentDialogService.DialogResult> ResultSource) operation))
            {
                if (operation.ResultSource.Task.IsCompleted)
                {
                    continue;
                }

                await _available.Task.ConfigureAwait(true);

                _available = new TaskCompletionSource<bool>();

                topLevel.KeyDown += OnContentDialogKeyDown;
                Tag = operation.Content;
                _contentDialogResult = operation.ResultSource;
                Opacity = 1;

                if (content != operation.Content || resultSource != operation.ResultSource)
                {
                    break;
                }

                try
                {
                    result = await operation.ResultSource.Task.ConfigureAwait(true);
                }
                finally
                {
                    Opacity = 0;
                    topLevel.KeyDown -= OnContentDialogKeyDown;
                    _contentDialogResult = null;
                }
            }
        }

        return result;
    }
}