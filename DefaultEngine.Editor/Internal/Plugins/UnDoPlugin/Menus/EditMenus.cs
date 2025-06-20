using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using DefaultEngine.Editor.Api;
using DefaultEngine.Editor.Api.Services;
using DefaultUnDo;
using Microsoft.Extensions.Logging;

namespace DefaultEngine.Editor.Internal.Plugins.UnDoPlugin.Menus;

internal sealed class EditMenus : IMenu
{
    public int Order => int.MinValue + 1;

    public IReadOnlyList<string> Path { get; } = ["Edit"];
}

internal sealed class UndoMenu : ICommandMenu
{
    private readonly IUnDoManager _manager;

    public int Order => 0;

    public IReadOnlyList<string> Path { get; } = ["Edit", "Undo"];

    public object? Icon { get; } = new StaticResourceExtension("UnDoPlugin.UndoIcon");

    public KeyGesture HotKey { get; } = new(Key.Z, KeyModifiers.Control);

    public UndoMenu(IUnDoManager manager, ILogger<UndoMenu> logger)
    {
        _manager = manager;
        manager.Do(() => logger.LogInformation("do"), () => logger.LogInformation("undo"));
    }

    public bool CanExecute() => _manager.CanUndo;

    public void Execute() => _manager.Undo();
}

internal sealed class RedoMenu : ICommandMenu
{
    private readonly IUnDoManager _manager;

    public int Order => 1;

    public IReadOnlyList<string> Path { get; } = ["Edit", "Redo"];

    public object? Icon { get; } = new StaticResourceExtension("UnDoPlugin.RedoIcon");

    public KeyGesture HotKey { get; } = new(Key.Y, KeyModifiers.Control);

    public RedoMenu(IUnDoManager manager)
    {
        _manager = manager;
    }

    public bool CanExecute() => _manager.CanRedo;

    public void Execute() => _manager.Redo();
}

internal sealed class TestMenu : IAsyncCommandMenu
{
    private readonly IContentDialogService _service;

    public IReadOnlyList<string> Path { get; } = ["test"];

    public TestMenu(IContentDialogService service)
    {
        _service = service;
    }

    public Task ExecuteAsync() => _service.ShowAsync("kikoo", true);
}