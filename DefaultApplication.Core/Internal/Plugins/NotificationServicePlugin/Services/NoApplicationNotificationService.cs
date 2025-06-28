using System;
using System.Threading.Tasks;
using DefaultApplication.Services;

namespace DefaultApplication.Internal.Plugins.NotificationServicePlugin.Services;

internal sealed class NoApplicationNotificationService : INotificationService
{
    public Task CloseAllAsync() => Task.CompletedTask;

    public Task ShowAsync(object content, INotificationService.NotificationType notificationType, TimeSpan? expiration = null) => Task.CompletedTask;
}
