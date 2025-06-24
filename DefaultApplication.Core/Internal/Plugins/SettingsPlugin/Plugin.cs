using System;
using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    private readonly PluginsHelper _plugins;

    public Plugin(Application application, PluginsHelper pluginsHelper)
    {
        Uri baseUri = new("avares://DefaultApplication.Core");
        Uri resourcesUri = new(baseUri, "Internal/Plugins/SettingsPlugin/Resources/");

        Dispatcher.UIThread.Invoke(() => application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") }));

        _plugins = pluginsHelper;
    }

    public void Register(IServiceCollection services)
    {
        services.TryAddTransient<SettingsViewModel>();

        foreach (Type type in _plugins.GetPluginsTypes().GetInstanciableImplementation<ISettings>())
        {
            services.AddAsSingletonImplementation<ISettings>(type);
        }
    }
}