using DefaultApplication.Plugins;
using DefaultApplication.Services;
using DefaultApplication.Internal.Plugins.NotificationsPlugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.NotificationsPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services) => services.TryAddSingleton<INotificationService, NotificationService>();
}
