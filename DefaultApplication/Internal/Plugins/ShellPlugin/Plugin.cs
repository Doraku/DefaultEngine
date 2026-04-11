using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.ShellPlugin;

internal sealed class Plugin : IServiceRegisterer
{
    private readonly Application? _application;

    public Plugin(Application? application = null)
    {
        _application = application;
    }

    public void Register(IServiceCollection services)
    {
        if (_application is { })
        {
            Uri baseUri = new("avares://DefaultApplication");
            Uri resourcesUri = new(baseUri, "/Internal/Plugins/ShellPlugin/Resources/");

            _application.Dispatcher.Invoke(() => _application.Styles.Add(new StyleInclude(baseUri) { Source = new Uri(resourcesUri, "Styles.axaml") }));
        }

        services.TryAddSingleton<ShellViewModel>();
    }
}
