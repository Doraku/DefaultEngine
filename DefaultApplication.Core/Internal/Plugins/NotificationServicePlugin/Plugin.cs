using Avalonia;
using DefaultApplication.Internal.Plugins.NotificationServicePlugin.Services;
using DefaultApplication.Plugins;
using DefaultApplication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.NotificationServicePlugin;

internal sealed class Plugin : IServiceRegisterer
{
    private readonly Application? _application;

    public Plugin(Application? application = null)
    {
        _application = application;
    }

    public void Register(IServiceCollection services)
    {
        if (_application is { })
        {
            services.TryAddSingleton<INotificationService, NotificationService>();
        }
        else
        {
            services.TryAddSingleton<INotificationService, NoApplicationNotificationService>();
        }
    }
}
