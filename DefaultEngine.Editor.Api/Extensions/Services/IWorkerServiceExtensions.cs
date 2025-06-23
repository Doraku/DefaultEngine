using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DefaultEngine.Editor.Api.Services;

public static class IWorkerServiceExtensions
{
    public static Task<IWorkerService.IOperationResult> ExecuteAsync(this IWorkerService service, Func<CancellationToken, Task> task, [CallerMemberName] string? operationName = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(operation =>
        {
            operation.Name = operationName;

            return task(operation.CancellationToken);
        });
    }

    public static Task<IWorkerService.IOperationResult<T>> ExecuteAsync<T>(this IWorkerService service, Func<CancellationToken, Task<T>> task, [CallerMemberName] string? operationName = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(operation =>
        {
            operation.Name = operationName;

            return task(operation.CancellationToken);
        });
    }

    public static Task<IWorkerService.IOperationResult> ExecuteAsync(this IWorkerService service, Func<Task> task, [CallerMemberName] string? operationName = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(operation =>
        {
            operation.Name = operationName;

            return task();
        });
    }

    public static Task<IWorkerService.IOperationResult<T>> ExecuteAsync<T>(this IWorkerService service, Func<Task<T>> task, [CallerMemberName] string? operationName = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(operation =>
        {
            operation.Name = operationName;

            return task();
        });
    }
}
