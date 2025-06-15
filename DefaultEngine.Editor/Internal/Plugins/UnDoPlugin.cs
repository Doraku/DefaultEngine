using DefaultEngine.Editor.Api.Plugins;
using DefaultUnDo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins;

internal sealed class UnDoPlugin : IServicesRegisterer
{
    public void Register(IServiceCollection services) => services.TryAddSingleton<IUnDoManager, UnDoManager>();
}
