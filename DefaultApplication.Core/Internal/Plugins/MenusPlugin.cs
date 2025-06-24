using System;
using System.Reflection;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.Internal.Plugins;

internal sealed class MenusPlugin : IServicesRegisterer
{
    private readonly PluginsHelper _plugins;

    public MenusPlugin(PluginsHelper plugins)
    {
        _plugins = plugins;
    }

    public void Register(IServiceCollection services)
    {
        foreach (Type type in _plugins.GetPluginsTypes().GetInstanciableImplementation<IMenu>())
        {
            services.AddAsSingletonImplementation<IMenu>(type);
        }
    }
}
