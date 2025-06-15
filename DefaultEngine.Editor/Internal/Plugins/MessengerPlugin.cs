using CommunityToolkit.Mvvm.Messaging;
using DefaultEngine.Editor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins;

internal sealed class MessengerPlugin : IServicesRegisterer
{
    public void Register(IServiceCollection services) => services.TryAddSingleton<IMessenger, WeakReferenceMessenger>();
}
