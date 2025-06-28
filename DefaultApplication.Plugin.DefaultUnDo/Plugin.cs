using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultApplication.Plugins;
using DefaultUnDo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Plugin.DefaultUnDo;

internal sealed class Plugin : IServiceRegisterer
{
    public Plugin(Application? application = null)
    {
        if (application is null)
        {
            return;
        }

        Uri baseUri = new("avares://DefaultApplication.Plugin.DefaultUnDo");
        Uri resourcesUri = new(baseUri, "Resources/");

        Dispatcher.UIThread.Invoke(() =>
        {
            application.Styles.Add(new StyleInclude(baseUri) { Source = new Uri(resourcesUri, "Styles.axaml") });

            application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") });
        });
    }

    public void Register(IServiceCollection services) => services.TryAddSingleton<IUnDoManager, UnDoManager>();
}
