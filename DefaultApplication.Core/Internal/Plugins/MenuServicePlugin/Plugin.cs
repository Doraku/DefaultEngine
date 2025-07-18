using System;
using System.Reflection;
using DefaultApplication.Internal.Plugins.MenuServicePlugin.Services;
using DefaultApplication.Plugins;
using DefaultApplication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.MenuServicePlugin;

internal sealed class Plugin : IServiceRegisterer
{
    private readonly PluginsHelper _plugins;

    public Plugin(PluginsHelper plugins)
    {
        _plugins = plugins;
    }

    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<IMenuService, MenuService>();

        foreach (Type type in _plugins.GetTypes().GetInstanciableImplementation<IMenu>())
        {
            services.AddAsSingletonImplementation<IMenu>(type);
        }
    }
}
