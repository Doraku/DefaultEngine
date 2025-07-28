using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using DefaultApplication.DependencyInjection;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Menus;

internal sealed class FileMenu : IMenu
{
    public int Order => int.MinValue;

    public IReadOnlyList<string> Path { get; } = ["File"];
}

internal sealed class ExitMenu : IAsyncCommandMenu
{
    private readonly IDelayed<TopLevel> _mainTopLevel;

    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["File", "Exit"];

    public KeyGesture HotKey { get; } = new(Key.F4, KeyModifiers.Alt);

    public ExitMenu(IDelayed<TopLevel> mainTopLevel)
    {
        _mainTopLevel = mainTopLevel;
    }

    public async Task ExecuteAsync() => ((Window)await _mainTopLevel.Task.ConfigureAwait(true)).Close();
}
