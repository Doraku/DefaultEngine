using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.DockingLayout.Internal;

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
            Uri baseUri = new("avares://DefaultApplication.Plugin.DockingLayout");
            Uri resourcesUri = new(baseUri, "Internal/Resources/");

            Dispatcher.UIThread.Invoke(() =>
            {
                _application.Styles.Add(new StyleInclude(baseUri) { Source = new Uri(resourcesUri, "Styles.axaml") });

                _application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") });
            });

            services.AddSingleton<IDockingLayoutService, DockingLayoutService>();
        }
        else
        {
            services.AddSingleton<IDockingLayoutService, NoApplicationDockingLayoutService>();
        }
    }
}
