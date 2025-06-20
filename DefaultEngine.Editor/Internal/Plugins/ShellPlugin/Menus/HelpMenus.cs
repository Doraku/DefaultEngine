using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultEngine.Editor.Api;
using DefaultEngine.Editor.Api.Services;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Menus;

internal sealed class HelpMenu : IMenu
{
    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["Help"];
}

internal sealed class AboutMenu : IAsyncCommandMenu
{
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _provider;

    public int Order => int.MaxValue;

    public object? Icon => new Uri("avares://DefaultEngine.Editor/Resources/Images/DefaultLogo.png");

    public IReadOnlyList<string> Path { get; } = ["Help", "About"];

    public AboutMenu(IContentDialogService contentDialogService, IServiceProvider provider)
    {
        _contentDialogService = contentDialogService;
        _provider = provider;
    }

    public Task ExecuteAsync() => _contentDialogService.ShowAsync(_provider.GetRequiredService<AboutViewModel>());
}