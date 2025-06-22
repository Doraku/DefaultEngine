using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using static DefaultEngine.Editor.Api.Services.IWorkerService;

namespace DefaultEngine.Editor.Internal.Plugins.WorkerServicePlugin.ViewModels;

internal sealed partial class OperationViewModel : ObservableObject, ICancellableOperation, IDisposable
{
    private readonly CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private double _maximumProgress;

    [ObservableProperty]
    private double _currentProgress;

    public bool IsCancellable => _cancellationTokenSource is { };

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    public OperationViewModel(CancellationTokenSource? cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
    }

    [RelayCommand(CanExecute = nameof(IsCancellable))]
    private Task OnCancel() => _cancellationTokenSource?.CancelAsync() ?? Task.CompletedTask;

    public void Dispose() => _cancellationTokenSource?.Dispose();
}
