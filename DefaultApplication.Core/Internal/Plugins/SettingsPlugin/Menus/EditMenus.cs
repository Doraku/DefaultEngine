﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.MarkupExtensions;
using DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;
using DefaultApplication.Services;
using DefaultApplication.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin.Menus;

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

        using IServiceScope scope = _provider.CreateScope();

        if (await _contentDialogService.ShowAsync(scope.ServiceProvider.GetRequiredService<SettingsViewModel>()).ConfigureAwait(false) != IContentDialogService.DialogResult.Primary)
        {
            foreach (ISettings settings in _provider.GetRequiredService<IEnumerable<ISettings>>())
            {
                settings.Read();
            }
        }
        else
        {
            foreach (ISettings settings in _provider.GetRequiredService<IEnumerable<ISettings>>())
            {
                settings.Write();
            }
        }
    }
}
