using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed class Plugin : IServiceRegisterer
{
    public Plugin(Application? application = null)
    {
        if (application is null)
        {
            return;
        }

        Uri baseUri = new("avares://DefaultApplication.Plugin.DockingLayout");
        Uri resourcesUri = new(baseUri, "Resources/");

        Dispatcher.UIThread.Invoke(() => application.Styles.Add(new StyleInclude(baseUri) { Source = new Uri(resourcesUri, "Styles.axaml") }));
    }

    public void Register(IServiceCollection services)
    {
        services.AddSingleton<IDockingLayoutService, DockingLayoutService>();
    }
}
