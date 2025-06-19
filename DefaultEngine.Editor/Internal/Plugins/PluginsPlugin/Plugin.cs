using System;
using System.Reflection;
using DefaultEngine.Editor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor.Internal.Plugins.PluginsPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    private readonly PluginsHelper _plugins;

    public Plugin(PluginsHelper plugins)
    {
        _plugins = plugins;
    }

    public void Register(IServiceCollection services)
    {
        foreach (Type type in _plugins.GetPluginsTypes().GetInstanciableImplementation<IPlugin>())
        {
            services.AddAsSingletonImplementation<IPlugin>(type);
        }
    }
}
