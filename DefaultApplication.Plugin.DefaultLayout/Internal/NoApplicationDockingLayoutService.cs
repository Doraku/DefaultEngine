using System;
using System.Threading.Tasks;
using Avalonia.DefaultLayout;

namespace DefaultApplication.DefaultLayout.Internal;

internal sealed class NoApplicationDockingLayoutService : IDockingLayoutService
{
    public Task<ILayoutContent> ShowAsync<T>(LayoutOptions options, T content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return Task.FromResult<ILayoutContent>(new LayoutContent(options, content));
    }

    public Task CloseAsync(ILayoutContent content) => Task.CompletedTask;
}
