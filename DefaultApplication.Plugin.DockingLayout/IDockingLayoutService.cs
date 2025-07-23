namespace DefaultApplication.DockingLayout;

public interface IDockingLayoutService
{
    enum DockableType
    {
        Document,
        Tool
    }

    void Show<T>(DockableType dockableType) where T : notnull;

    void Show<T>(T content, DockableType dockableType);
}
