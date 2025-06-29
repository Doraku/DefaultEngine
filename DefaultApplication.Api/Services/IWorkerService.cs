using System;
using System.Threading;
using System.Threading.Tasks;

namespace DefaultApplication.Services;

public interface IWorkerService
{
    interface IOperation
    {
        object? Header { get; set; }

        object? Content { get; set; }

        bool HasError { get; set; }

        double MaximumProgress { get; set; }

        double CurrentProgress { get; set; }
    }

    interface IOperationResult
    {
        Exception? Exception { get; }

        bool IsCompleted => Exception is null;

        bool IsCanceled => Exception is OperationCanceledException;

        bool IsFaulted => Exception is { };
    }

    interface IOperationResult<T> : IOperationResult
    {
        T? Result { get; }
    }

    interface ICancellableOperation : IOperation
    {
        CancellationToken CancellationToken { get; }
    }

    Task<IOperationResult> ExecuteAsync(Func<IOperation, Task> task);

    Task<IOperationResult> ExecuteAsync(Func<ICancellableOperation, Task> task);

    Task<IOperationResult<T>> ExecuteAsync<T>(Func<IOperation, Task<T>> task);

    Task<IOperationResult<T>> ExecuteAsync<T>(Func<ICancellableOperation, Task<T>> task);
}
