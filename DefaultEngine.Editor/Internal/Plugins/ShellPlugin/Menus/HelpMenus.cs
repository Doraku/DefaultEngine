using System;
using System.Collections.Generic;
using DefaultEngine.Editor.Api;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Menus;

internal sealed class HelpMenus : IMenu
{
    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["Help"];
}

internal sealed class AboutMenu : ICommandMenu
{
    public int Order => int.MaxValue;

    public object? Icon => new Uri("avares://DefaultEngine.Editor/Resources/Images/DefaultLogo.png");

    public IReadOnlyList<string> Path { get; } = ["Help", "About"];

    public void Execute() { }
}