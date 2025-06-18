using System.Collections.Generic;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;
using DefaultEngine.Editor.Api;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Menus;

internal sealed class FileMenu : IMenu
{
    public int Order => int.MinValue;

    public IReadOnlyList<string> Path { get; } = ["File"];
}

internal sealed class ExitMenu : ICommandMenu
{
    internal sealed record Message;

    private readonly IMessenger _messenger;

    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["File", "Exit"];

    public KeyGesture HotKey { get; } = new(Key.F4, KeyModifiers.Alt);

    public ExitMenu(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public void Execute() => _messenger.Send<Message>();
}
