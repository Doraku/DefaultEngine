namespace DefaultApplication.DockingLayout;

public interface IDockingLayoutService
{
    void Show<T>(T content, LayoutOptions dockableType);
}
