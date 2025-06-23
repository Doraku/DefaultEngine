using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using DefaultEngine.Editor.Api.DependencyInjection;
using DefaultEngine.Editor.Api.Services;

namespace DefaultEngine.Editor.Internal.Plugins.NotificationsPlugin.Services;

internal sealed class NotificationService : INotificationService
{
    private readonly Lazy<Task<WindowNotificationManager>> _manager;

    public NotificationService(IDelayedItem<Window> mainWindow)
    {
        _manager = new Lazy<Task<WindowNotificationManager>>(async () =>
        {
            TopLevel topLevel = await mainWindow.Value.ConfigureAwait(false);

            return await Dispatcher.UIThread.InvokeAsync(() => new WindowNotificationManager(topLevel) { Position = NotificationPosition.BottomRight });
        });
    }

    private static NotificationType AsAvalonia(INotificationService.NotificationType notificationType)
        => notificationType switch
        {
            INotificationService.NotificationType.Information => NotificationType.Information,
            INotificationService.NotificationType.Success => NotificationType.Success,
            INotificationService.NotificationType.Warning => NotificationType.Warning,
            INotificationService.NotificationType.Error => NotificationType.Error,
            _ => NotificationType.Information
        };

    public async Task ShowAsync(object content, INotificationService.NotificationType notificationType, TimeSpan? expiration = null)
    {
        WindowNotificationManager manager = await _manager.Value.ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(() => manager.Show(content, AsAvalonia(notificationType), expiration));
    }

    public async Task CloseAllAsync()
    {
        WindowNotificationManager manager = await _manager.Value.ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(manager.CloseAll);
    }
}
