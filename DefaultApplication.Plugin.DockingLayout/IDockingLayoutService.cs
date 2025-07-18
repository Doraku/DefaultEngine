namespace DefaultApplication.DockingLayout;

public interface IDockingLayoutService
{
    void Show<T>() where T : notnull;

    void Show<T>(T content);
}
