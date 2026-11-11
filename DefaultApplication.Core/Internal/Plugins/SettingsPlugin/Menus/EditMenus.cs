using System;
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
    private readonly IContentDialogService _dialog;
    private readonly IWorkerService _worker;
    private readonly IServiceProvider _provider;

    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["Edit", "Settings"];

    public object? Icon { get; } = new StaticResourceExtension("SettingsPlugin.SettingsIcon");

    public SettingsMenu(IContentDialogService dialog, IWorkerService worker, IServiceProvider provider)
    {
        _dialog = dialog;
        _worker = worker;
        _provider = provider;
    }

    public async Task ExecuteAsync()
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        using IServiceScope scope = _provider.CreateScope();

        if (await _dialog.ShowAsync(scope.ServiceProvider.GetRequiredService<SettingsViewModel>()).ConfigureAwait(false) != IContentDialogService.DialogResult.Primary)
        {
            await _worker.ExecuteAsync(async operation =>
            {
                operation.Header = "Reloading settings...";
                foreach (ISettings settings in _provider.GetRequiredService<IEnumerable<ISettings>>())
                {
                    await settings.ReadAsync(operation.CancellationToken).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }
        else
        {
            await _worker.ExecuteAsync(async operation =>
            {
                operation.Header = "Saving settings...";
                foreach (ISettings settings in _provider.GetRequiredService<IEnumerable<ISettings>>())
                {
                    await settings.WriteAsync(operation.CancellationToken).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }
    }
}
