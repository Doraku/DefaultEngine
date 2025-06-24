using Avalonia.Threading;
using DefaultApplication.Internal.Plugins.ContentDialogServicePlugin.Controls;
using DefaultApplication.Plugins;
using DefaultApplication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.ContentDialogServicePlugin;

internal sealed class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<ContentDialogControl>();
        services.TryAddSingleton<IContentDialogService>(provider => Dispatcher.UIThread.Invoke(provider.GetRequiredService<ContentDialogControl>));
    }
}
