namespace DefaultApplication.DockingLayout.Internal;

internal sealed class NoApplicationDockingLayoutService : IDockingLayoutService
{
    public void Show<T>() where T : notnull
    { }

    public void Show<T>(T content)
    { }
}
