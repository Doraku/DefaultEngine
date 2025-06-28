using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultApplication.Internal.Plugins.AboutPlugin.ViewModels;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.AboutPlugin;

internal sealed class Plugin : IServiceRegisterer
{
    public Plugin(Application? application = null)
    {
        if (application is null)
        {
            return;
        }

        Uri baseUri = new("avares://DefaultApplication.Core");
        Uri resourcesUri = new(baseUri, "Internal/Plugins/AboutPlugin/Resources/");

        Dispatcher.UIThread.Invoke(() => application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") }));
    }

    public void Register(IServiceCollection services)
    {
        services.TryAddTransient<AboutViewModel>();
    }
}
