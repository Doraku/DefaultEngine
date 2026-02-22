using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

using static DefaultApplication.Services.IWorkerService;

namespace DefaultApplication.Internal.Plugins.WorkerServicePlugin.ViewModels;

internal sealed class OperationViewModel : INotifyPropertyChanged, ICancellableOperation, IDisposable
{
    private readonly CancellationTokenSource? _cancellationTokenSource;

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    public bool IsCancellable => _cancellationTokenSource is { IsCancellationRequested: false };

    public object? Header
    {
        get;
        set => SetProperty(ref field, value);
    }

    public object? Content
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool HasError
    {
        get;
        set => SetProperty(ref field, value);
    }

    public double MaximumProgress
    {
        get;
        set => SetProperty(ref field, value);
    }

    public double CurrentProgress
    {
        get;
        set => SetProperty(ref field, value);
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
