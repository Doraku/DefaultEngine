using System;
using System.Reflection;
using DefaultEngine.Editor.Api;
using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    private readonly PluginsHelper _plugins;

    public Plugin(PluginsHelper pluginsHelper)
    {
        _plugins = pluginsHelper;
    }

    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<ShellViewModel>();

        foreach (Type type in _plugins.GetPluginsTypes().GetInstanciableImplementation<IMenu>())
        {
            services.TryAddSingleton(type);
            services.AddSingleton(typeof(IMenu), provider => provider.GetRequiredService(type));
        }
    }
}
