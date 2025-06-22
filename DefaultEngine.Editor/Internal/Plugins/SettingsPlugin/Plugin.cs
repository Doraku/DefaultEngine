using System;
using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultEngine.Editor.Api;
using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins.SettingsPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    private readonly PluginsHelper _plugins;

    public Plugin(Application application, PluginsHelper pluginsHelper)
    {
        Uri baseUri = new("avares://DefaultEngine.Editor");
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