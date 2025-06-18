using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using DefaultEngine.Editor.Api;
using DefaultUnDo;

namespace DefaultEngine.Editor.Internal.Plugins.UnDoPlugin.Menus;

internal sealed class EditMenus : IMenu
{
    public int Order => int.MinValue + 1;

    public IReadOnlyList<string> Path { get; } = ["Edit"];
}

internal sealed class Undo : ICommandMenu
{
    private readonly IUnDoManager _manager;

    public int Order => 0;

    public IReadOnlyList<string> Path { get; } = ["Edit", "Undo"];

    public object? Icon { get; } = new StaticResourceExtension("UnDoPlugin.UndoIcon");

    public KeyGesture HotKey { get; } = new(Key.Z, KeyModifiers.Control);

    public Undo(IUnDoManager manager)
    {
        _manager = manager;
    }

    public bool CanExecute() => _manager.CanUndo;

    public void Execute() => _manager.Undo();
}

internal sealed class Redo : ICommandMenu
{
    private readonly IUnDoManager _manager;

    public int Order => 1;

    public IReadOnlyList<string> Path { get; } = ["Edit", "Redo"];

    public object? Icon { get; } = new StaticResourceExtension("UnDoPlugin.RedoIcon");

    public KeyGesture HotKey { get; } = new(Key.Y, KeyModifiers.Control);

    public Redo(IUnDoManager manager)
    {
        _manager = manager;
    }

    public bool CanExecute() => _manager.CanRedo;

    public void Execute() => _manager.Redo();
}