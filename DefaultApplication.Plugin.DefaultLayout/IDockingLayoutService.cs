using System.Threading.Tasks;
using Avalonia.DefaultLayout;

namespace DefaultApplication.DefaultLayout;

public interface IDockingLayoutService
{
    Task<ILayoutContent> ShowAsync<T>(LayoutOptions options, T content);

    Task CloseAsync(ILayoutContent content);
}
