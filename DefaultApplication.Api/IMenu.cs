using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;

namespace DefaultApplication;

public interface IMenu
{
    int Order => 0;

    IReadOnlyList<string> Path { get; }

    object? Icon => null;
}

public interface IBaseCommandMenu : IMenu
{
    KeyGesture? HotKey => null;

    bool CanExecute() => true;
}

public interface ICommandMenu : IBaseCommandMenu
{
    void Execute();
}

public interface IAsyncCommandMenu : IBaseCommandMenu
{
    Task ExecuteAsync();
}