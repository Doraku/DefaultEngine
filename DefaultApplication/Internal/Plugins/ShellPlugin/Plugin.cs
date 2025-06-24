using Avalonia.Threading;
using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;
using DefaultApplication.Internal.Plugins.ShellPlugin.Views;
using DefaultApplication.Plugins;
using DefaultApplication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.ShellPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<ShellViewModel>();
        services.TryAddSingleton<IContentDialogService>(provider => Dispatcher.UIThread.Invoke(provider.GetRequiredService<ShellView>));
    }
}
