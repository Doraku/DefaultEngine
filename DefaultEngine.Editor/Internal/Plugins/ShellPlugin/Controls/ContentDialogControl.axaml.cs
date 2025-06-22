using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using DefaultEngine.Editor.Api.Controls.Behaviors;
using DefaultEngine.Editor.Api.Services;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Controls;

internal sealed partial class ContentDialogControl : Border
{
    private readonly Stack<(object, TaskCompletionSource<IContentDialogService.DialogResult>)> _operations;

    private TaskCompletionSource<bool> _available;
    private TaskCompletionSource<IContentDialogService.DialogResult>? _contentDialogResult;

    public ContentDialogControl()
    {
        _operations = [];

        _available = new TaskCompletionSource<bool>();
        _available.SetResult(true);

        InitializeComponent();
    }

    [RelayCommand]
    public void OnContentDialogReturn(IContentDialogService.DialogResult result) => _contentDialogResult?.TrySetResult(result);

    private void OnContentDialogKeyDown(object? sender, KeyEventArgs e)
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

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsVisibleProperty
            && !e.GetNewValue<bool>())
        {
            Tag = null;
            _available.TrySetResult(true);
        }
    }

    public async Task<IContentDialogService.DialogResult> ShowAsync(object content, CancellationToken cancellationToken)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => ShowAsync(content, cancellationToken)).ConfigureAwait(false);
        }

        TopLevel? topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
        {
            return IContentDialogService.DialogResult.None;
        }

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