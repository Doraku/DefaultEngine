using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;

namespace DefaultEngine.Editor.Api.Plugins;

public interface ICommonMenuItem
{
    IReadOnlyList<string> Path { get; }

    object? Icon => null;

    KeyGesture? HotKey => null;

    bool CanExecute() => true;
}

public interface IMenuItem : ICommonMenuItem
{
    void Execute();
}

public interface IAsyncMenuItem : ICommonMenuItem
{
    Task ExecuteAsync();
}

public interface ICancellableAsyncMenuItem : ICommonMenuItem
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
