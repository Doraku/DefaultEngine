using System;
using System.Threading.Tasks;

namespace DefaultApplication.Services;

public static class INotificationServiceExtensions
{
    public static Task ShowInformationAsync(this INotificationService service, object content, TimeSpan? expiration = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(content);

        return service.ShowAsync(content, INotificationService.NotificationType.Information, expiration);
    }

    public static Task ShowSuccessAsync(this INotificationService service, object content, TimeSpan? expiration = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(content);

        return service.ShowAsync(content, INotificationService.NotificationType.Success, expiration);
    }

    public static Task ShowWarningAsync(this INotificationService service, object content, TimeSpan? expiration = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(content);

        return service.ShowAsync(content, INotificationService.NotificationType.Warning, expiration);
    }

    public static Task ShowErrorAsync(this INotificationService service, object content, TimeSpan? expiration = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(content);

        return service.ShowAsync(content, INotificationService.NotificationType.Error, expiration);
    }
}
