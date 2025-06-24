using System;
using System.Threading.Tasks;

namespace DefaultApplication.Services;

public interface INotificationService
{
    enum NotificationType
    {
        Information,
        Success,
        Warning,
        Error
    }

    Task ShowAsync(object content, NotificationType notificationType, TimeSpan? expiration = null);

    Task CloseAllAsync();
}
