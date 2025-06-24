using System;
using System.Threading;
using System.Threading.Tasks;
using DefaultApplication.Internal.Plugins.WorkerServicePlugin.ViewModels;
using DefaultApplication.Services;
using Microsoft.Extensions.Logging;
using static DefaultApplication.Services.IWorkerService;

namespace DefaultApplication.Internal.Plugins.WorkerServicePlugin.Services;

internal sealed class WorkerService : IWorkerService
{
    private sealed record OperationResult<T>(Exception? Exception = default, T? Result = default) : IOperationResult<T>;

    private readonly ILogger<WorkerService> _logger;
    private readonly IContentDialogService _contentDialogService;

    public WorkerService(ILogger<WorkerService> logger, IContentDialogService contentDialogService)
    {
        _logger = logger;
        _contentDialogService = contentDialogService;
    }

    private static Func<ICancellableOperation, Task<bool>> WrapTask(Func<ICancellableOperation, Task> task)
    {
        return async (operation) =>
        {
            await task(operation).ConfigureAwait(false);

            return true;
        };
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    private async Task<IOperationResult<T>> ExecuteAsync<T>(OperationViewModel operation, Func<ICancellableOperation, Task<T>> task)
    {
        using (operation)
        {
            using CancellationTokenSource contentDialogHandle = new();

            try
            {
                Task<T> operationTask = task(operation);
                Task contentDialogTask = _contentDialogService.ShowAsync(operation, contentDialogHandle.Token);

                try
                {
                    return new OperationResult<T>(Result: await operationTask.ConfigureAwait(false));
                }
                finally
                {
                    await contentDialogHandle.CancelAsync().ConfigureAwait(false);
                    await contentDialogTask.ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                _logger.LogWorkerServiceException(operation, exception);

                return new OperationResult<T>(Exception: exception);
            }
        }
    }

    public async Task<IOperationResult> ExecuteAsync(Func<IOperation, Task> task) => await ExecuteAsync(new OperationViewModel(null), WrapTask(task)).ConfigureAwait(false);

    public async Task<IOperationResult> ExecuteAsync(Func<ICancellableOperation, Task> task) => await ExecuteAsync(new OperationViewModel(new CancellationTokenSource()), WrapTask(task)).ConfigureAwait(false);

    public async Task<IOperationResult<T>> ExecuteAsync<T>(Func<IOperation, Task<T>> task) => await ExecuteAsync(new OperationViewModel(null), task).ConfigureAwait(false);

    public async Task<IOperationResult<T>> ExecuteAsync<T>(Func<ICancellableOperation, Task<T>> task) => await ExecuteAsync(new OperationViewModel(new CancellationTokenSource()), task).ConfigureAwait(false);
}
