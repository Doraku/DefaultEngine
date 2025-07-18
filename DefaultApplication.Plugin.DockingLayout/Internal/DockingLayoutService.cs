using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed class DockingLayoutService : IDockingLayoutService
{
    private readonly IServiceProvider _provider;

    public DockingLayoutService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void Show<T>() where T : notnull => Show(_provider.GetRequiredService<T>());

    public void Show<T>(T content)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => Show(content));

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
