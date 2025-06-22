using System;
using System.Threading;
using System.Threading.Tasks;

namespace DefaultEngine.Editor.Api.Services;

public static class IWorkerServiceExtensions
{
    public static Task<IWorkerService.IOperationResult> ExecuteAsync(this IWorkerService service, Func<CancellationToken, Task> task)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(operation => task(operation.CancellationToken));
    }

    public static Task<IWorkerService.IOperationResult<T>> ExecuteAsync<T>(this IWorkerService service, Func<CancellationToken, Task<T>> task)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(operation => task(operation.CancellationToken));
    }

    public static Task<IWorkerService.IOperationResult> ExecuteAsync(this IWorkerService service, Func<Task> task)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(_ => task());
    }

    public static Task<IWorkerService.IOperationResult<T>> ExecuteAsync<T>(this IWorkerService service, Func<Task<T>> task)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(task);

        return service.ExecuteAsync(_ => task());
    }
}
