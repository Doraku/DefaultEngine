using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

using static DefaultApplication.Services.IWorkerService;

namespace DefaultApplication.Internal.Plugins.WorkerServicePlugin.ViewModels;

internal sealed class OperationViewModel : INotifyPropertyChanged, ICancellableOperation, IDisposable
{
    private readonly CancellationTokenSource? _cancellationTokenSource;

    private string? _name;
    private bool _hasError;
    private double _maximumProgress;
    private double _currentProgress;

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    public bool IsCancellable => _cancellationTokenSource is { IsCancellationRequested: false };

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public double MaximumProgress
    {
        get => _maximumProgress;
        set => SetProperty(ref _maximumProgress, value);
    }

    public double CurrentProgress
    {
        get => _currentProgress;
        set => SetProperty(ref _currentProgress, value);
    }

    public OperationViewModel(CancellationTokenSource? cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        NotifyPropertyChanged(nameof(IsCancellable));
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;

        NotifyPropertyChanged(propertyName);
    }

    #endregion

    #region IDisposable

    public void Dispose() => _cancellationTokenSource?.Dispose();

    #endregion
}
