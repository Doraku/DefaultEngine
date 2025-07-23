using static DefaultApplication.DockingLayout.IDockingLayoutService;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed class NoApplicationDockingLayoutService : IDockingLayoutService
{
    public void Show<T>(DockableType dockableType) where T : notnull
    { }

    public void Show<T>(T content, DockableType dockableType)
    { }
}
