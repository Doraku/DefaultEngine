using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Input;

namespace DefaultApplication.Services;

public interface IMenuService
{
    interface IMenuCommand : ICommand
    {
        string Header { get; }

        object? Icon { get; }

        KeyGesture? HotKey { get; }

        IReadOnlyCollection<IMenuCommand>? SubCommands { get; }

        bool CanExecute();

        Task ExecuteAsync();
    }

    bool IsEnabled { get; set; }

    IReadOnlyCollection<IMenuCommand> Commands { get; }
}
