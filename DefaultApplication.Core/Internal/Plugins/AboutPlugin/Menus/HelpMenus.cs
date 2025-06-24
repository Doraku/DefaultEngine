using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.MarkupExtensions;
using DefaultApplication.Internal.Plugins.AboutPlugin.ViewModels;
using DefaultApplication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.Internal.Plugins.AboutPlugin.Menus;

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

    public object? Icon => new StaticResourceExtension("AboutPlugin.AboutIcon");

    public IReadOnlyList<string> Path { get; } = ["Help", "About"];

    public AboutMenu(IContentDialogService contentDialogService, IServiceProvider provider)
    {
        _contentDialogService = contentDialogService;
        _provider = provider;
    }

    public async Task ExecuteAsync()
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        using IServiceScope scope = _provider.CreateScope();

        await _contentDialogService.ShowAsync(scope.ServiceProvider.GetRequiredService<AboutViewModel>()).ConfigureAwait(false);
    }
}