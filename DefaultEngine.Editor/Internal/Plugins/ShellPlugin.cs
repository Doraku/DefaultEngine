using System;
using System.Reflection;
using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Internal.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins;

internal sealed class ShellPlugin : IServicesRegisterer
{
    private readonly PluginsHelper _plugins;

    public ShellPlugin(PluginsHelper pluginsHelper)
    {
        _plugins = pluginsHelper;
    }

    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<ShellViewModel>();

        foreach (Type type in _plugins.GetPluginsTypes().GetInstanciableImplementation<ICommonMenuItem>())
        {
            services.TryAddSingleton(type);
            services.AddSingleton(typeof(ICommonMenuItem), provider => provider.GetRequiredService(type));
        }
    }
}
