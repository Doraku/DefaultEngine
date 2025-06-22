using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.MarkupExtensions;
using DefaultEngine.Editor.Api;
using DefaultEngine.Editor.Api.Services;
using DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.Menus;

internal sealed class EditMenus : IMenu
{
    public int Order => int.MinValue + 1;

    public IReadOnlyList<string> Path { get; } = ["Edit"];
}

internal sealed class SettingsMenu : IAsyncCommandMenu
{
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _provider;

    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["Edit", "Settings"];

    public object? Icon { get; } = new StaticResourceExtension("SettingsPlugin.SettingsIcon");

    public SettingsMenu(IContentDialogService contentDialogService, IServiceProvider provider)
    {
        _contentDialogService = contentDialogService;
        _provider = provider;
    }

    public async Task ExecuteAsync()
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        await _contentDialogService.ShowAsync(_provider.GetRequiredService<SettingsViewModel>()).ConfigureAwait(false);
    }
}
