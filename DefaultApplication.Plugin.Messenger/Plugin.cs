using CommunityToolkit.Mvvm.Messaging;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Plugin.Messenger;

internal sealed class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services) => services.TryAddSingleton<IMessenger, WeakReferenceMessenger>();
}
