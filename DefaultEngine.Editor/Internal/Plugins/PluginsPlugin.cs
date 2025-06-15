using System;
using System.Reflection;
using DefaultEngine.Editor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins;

internal sealed class PluginsPlugin : IServicesRegisterer
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
            services.TryAddSingleton(type);
            services.AddSingleton(typeof(IPlugin), provider => provider.GetRequiredService(type));
        }
    }
}
