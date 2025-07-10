using CommunityToolkit.Mvvm.Messaging;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Messenger;

internal sealed class Plugin : IServiceRegisterer
{
    public void Register(IServiceCollection services) => services.TryAddSingleton<IMessenger, WeakReferenceMessenger>();
}
