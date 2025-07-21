using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DefaultApplication.Controls.Behaviors;
using DefaultApplication.DependencyInjection;
using DefaultApplication.Services;

namespace DefaultApplication.Internal.Plugins.ContentDialogServicePlugin.Controls;

internal sealed partial class ContentDialogControl : Panel, IContentDialogService
{
    private readonly Task<TopLevel> _mainTopLevel;
    private readonly Stack<(object, TaskCompletionSource<IContentDialogService.DialogResult>)> _operations;

    private Layoutable? _target;
    private TaskCompletionSource _available;
    private TaskCompletionSource<IContentDialogService.DialogResult>? _contentDialogResult;

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

        _available = new TaskCompletionSource();
        _available.SetResult();

        InitializeComponent();
    }

    private void InstallOnTopLevel(TopLevel topLevel)
    {
        void OnTargetLayoutUpdated(object? sender, EventArgs e)
        {
            if (!IsVisible)
            {
                return;
            }

            if (sender is Layoutable layoutable)
            {
                Thickness margin = layoutable.Margin;
                double widthOffset = 0;
                double heightOffset = 0;

                if (sender is Window window && window.ExtendClientAreaToDecorationsHint)
                {
                    margin -= new Thickness(window.OffScreenMargin.Left / 2, window.OffScreenMargin.Top / 2, 0, 0);

                    widthOffset = window.OffScreenMargin.Left + window.OffScreenMargin.Right;
                    heightOffset = window.OffScreenMargin.Top + window.OffScreenMargin.Bottom;
                }

                Margin = new Thickness(layoutable.Bounds.Left - margin.Left, layoutable.Bounds.Top - margin.Top, 0, 0);
                MaxWidth = layoutable.Bounds.Width - widthOffset;
                MaxHeight = layoutable.Bounds.Height - heightOffset;
            }
        }

        void OnTopLevelTemplateApplied(object? sender, TemplateAppliedEventArgs e)
        {
            if (Parent is AdornerLayer adornerLayer)
            {
                adornerLayer.Children.Remove(this);
                AdornerLayer.SetAdornedElement(this, null);
            }

            TopLevel topLevel = (TopLevel)sender!;
            topLevel.TemplateApplied -= OnTopLevelTemplateApplied;
            if (_target is { })
            {
                _target.LayoutUpdated -= OnTargetLayoutUpdated;
            }

            InstallOnTopLevel(topLevel);
        }

        if (topLevel.FindDescendantOfType<VisualLayerManager>()?.AdornerLayer is AdornerLayer adorner)
        {
            _target = topLevel.GetVisualDescendants().OfType<Layoutable>().FirstOrDefault(ContentDialog.GetIsTarget) ?? topLevel;
            _target.LayoutUpdated += OnTargetLayoutUpdated;

            adorner.Children.Add(this);
            AdornerLayer.SetAdornedElement(this, adorner);
        }

        topLevel.TemplateApplied += OnTopLevelTemplateApplied;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty
            && !change.GetNewValue<bool>())
        {
            Tag = null;
            _available.TrySetResult();
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

                _available = new TaskCompletionSource();

                topLevel.KeyDown += OnContentDialogKeyDown;
                Tag = operation.Content;
                ContentDialog.SetIsVisible(_target, true);
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
                    if (_operations.Count is 0)
                    {
                        ContentDialog.SetIsVisible(_target, false);
                    }

                    Opacity = 0;
                    topLevel.KeyDown -= OnContentDialogKeyDown;
                    _contentDialogResult = null;
                }
            }
        }

        return result;
    }
}