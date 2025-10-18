using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.DefaultLayout;
using Avalonia.DefaultLayout.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DefaultApplication.DependencyInjection;

namespace DefaultApplication.DefaultLayout.Internal;

internal sealed class DockingLayoutService : IDockingLayoutService
{
    private readonly Task<LayoutControl> _root;

    public DockingLayoutService(IDelayed<TopLevel> mainTopLevel)
    {
        _root = mainTopLevel.Task.ContinueWith(
            async task =>
            {
                TopLevel topLevel = await task.ConfigureAwait(true);

                return topLevel.FindDescendantOfType<LayoutControl>() ?? throw new InvalidOperationException("No LayoutControl detected");
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default).Unwrap();
    }

    public async Task<ILayoutContent> ShowAsync<T>(LayoutOptions options, T content)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => ShowAsync(options, content)).ConfigureAwait(false);
        }

        LayoutControl root = await _root.ConfigureAwait(true);
        ILayoutContent layoutContent = new LayoutContent(options, content);

        root.Content = layoutContent;

        return layoutContent;
    }

    public async Task CloseAsync(ILayoutContent content)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (!Dispatcher.UIThread.CheckAccess())
        {
            await Dispatcher.UIThread.InvokeAsync(() => CloseAsync(content)).ConfigureAwait(false);
            return;
        }

        LayoutControl root = await _root.ConfigureAwait(true);
    }
}
