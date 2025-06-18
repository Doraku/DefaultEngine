using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultEngine.Editor.Api.Plugins;
using DefaultUnDo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins.UnDoPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    public Plugin(Application application)
    {
        Uri baseUri = new("avares://DefaultEngine.Editor");
        Uri resourcesUri = new(baseUri, "Internal/Plugins/UnDoPlugin/Resources/");

        Dispatcher.UIThread.Invoke(() =>
        {
            application.Styles.Add(new StyleInclude(baseUri) { Source = new Uri(resourcesUri, "Styles.axaml") });

            application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") });
        });
    }

    public void Register(IServiceCollection services) => services.TryAddSingleton<IUnDoManager, UnDoManager>();
}
