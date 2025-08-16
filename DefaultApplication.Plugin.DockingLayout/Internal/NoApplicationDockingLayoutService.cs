using Avalonia.DefaultLayout;

namespace DefaultApplication.DefaultLayout.Internal;

internal sealed class NoApplicationDockingLayoutService : IDockingLayoutService
{
    public void Show<T>(T content, LayoutOptions dockableType)
    { }
}
