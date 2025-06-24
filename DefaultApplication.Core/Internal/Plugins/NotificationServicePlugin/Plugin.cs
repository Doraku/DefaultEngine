using DefaultApplication.Internal.Plugins.NotificationsPlugin.Services;
using DefaultApplication.Plugins;
using DefaultApplication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.NotificationServicePlugin;

internal sealed class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services) => services.TryAddSingleton<INotificationService, NotificationService>();
}
