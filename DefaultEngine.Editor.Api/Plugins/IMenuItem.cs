using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DefaultEngine.Editor.Api.Plugins;

public interface IMenuItem
{
    IReadOnlyList<string> Path { get; }

    Task ExecuteAsync(CancellationToken cancellationToken);

    bool CanExecute();
}

public abstract class BaseMenuItem : BaseAsyncMenuItem
{
    protected BaseMenuItem(params ReadOnlySpan<string> path)
        : base(path)
    { }

    protected abstract void Execute();

    protected sealed override Task ExecuteAsync()
    {
        Execute();

        return Task.CompletedTask;
    }
}

public abstract class BaseAsyncMenuItem : BaseCancellableAsyncMenuItem
{
    protected BaseAsyncMenuItem(params ReadOnlySpan<string> path)
        : base(path)
    { }

    protected abstract Task ExecuteAsync();

    public sealed override Task ExecuteAsync(CancellationToken cancellationToken) => ExecuteAsync();
}

public abstract class BaseCancellableAsyncMenuItem : IMenuItem
{
    protected BaseCancellableAsyncMenuItem(params ReadOnlySpan<string> path)
    {
        Path = [.. path];
    }

    public IReadOnlyList<string> Path { get; }

    public virtual bool CanExecute() => true;

    public abstract Task ExecuteAsync(CancellationToken cancellationToken);
}
