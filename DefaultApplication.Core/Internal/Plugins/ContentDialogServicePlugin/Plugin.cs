using Avalonia;
using Avalonia.Threading;
using DefaultApplication.Internal.Plugins.ContentDialogServicePlugin.Controls;
using DefaultApplication.Internal.Plugins.ContentDialogServicePlugin.Services;
using DefaultApplication.Plugins;
using DefaultApplication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.ContentDialogServicePlugin;

internal sealed class Plugin : IServiceRegisterer
{
    private readonly Application? _application;

    public Plugin(Application? application = null)
    {
        _application = application;
    }

    public void Register(IServiceCollection services)
    {
        if (_application is { })
        {
            services.TryAddSingleton<ContentDialogControl>();
            services.TryAddSingleton<IContentDialogService>(provider => Dispatcher.UIThread.Invoke(provider.GetRequiredService<ContentDialogControl>));
        }
        else
        {
            services.TryAddSingleton<IContentDialogService, NoApplicationContentDialogService>();
        }
    }
}
