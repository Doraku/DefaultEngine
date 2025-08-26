using Avalonia.DefaultLayout;

namespace DefaultApplication.DefaultLayout;

public interface IDockingLayoutService
{
    void Show<T>(T content, LayoutOptions dockableType);
}
