using System;
using System.Reflection;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.Internal.Plugins;

internal sealed class PluginsPlugin : IServiceRegisterer
{
    private readonly PluginsHelper _plugins;

    public PluginsPlugin(PluginsHelper plugins)
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
