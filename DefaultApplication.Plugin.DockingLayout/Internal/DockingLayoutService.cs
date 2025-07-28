using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using DefaultApplication.DependencyInjection;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed class DockingLayoutService : IDockingLayoutService
{
    public DockingLayoutService(IDelayed<TopLevel> mainTopLevel)
    { }

    public void Show<T>(T content, LayoutOptions dockableType)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => Show(content, dockableType));

            return;
        }

        Window window = new()
        {
            Content = content
        };

#if DEBUG
        window.AttachDevTools();
#endif

        window.Classes.Add("Dockable");

        window.Show();
    }
}
