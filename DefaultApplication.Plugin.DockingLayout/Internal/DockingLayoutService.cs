using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using DefaultApplication.DependencyInjection;
using DefaultApplication.DockingLayout.Internal.Controls;
using Microsoft.Extensions.DependencyInjection;
using static DefaultApplication.DockingLayout.IDockingLayoutService;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed class DockingLayoutService : IDockingLayoutService
{
    private readonly IServiceProvider _provider;

    public DockingLayoutService(IServiceProvider provider, IDelayed<TopLevel> mainTopLevel)
    {
        _provider = provider;
    }

    public void Show<T>(DockableType dockableType) where T : notnull => Show(_provider.GetRequiredService<T>(), dockableType);

    public void Show<T>(T content, DockableType dockableType)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => Show(content, dockableType));

            return;
        }

        Window window = new()
        {
            Content = new DockableControl
            {
                DockableContent = content
            }
        };

#if DEBUG
        window.AttachDevTools();
#endif

        window.Classes.Add("Dockable");

        window.Show();
    }
}
