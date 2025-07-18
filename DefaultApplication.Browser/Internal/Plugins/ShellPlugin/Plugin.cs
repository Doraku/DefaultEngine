using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.ShellPlugin;

internal sealed class Plugin : IServiceRegisterer
{
    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<ShellViewModel>();
    }
}
