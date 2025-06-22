using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Api.Services;
using DefaultEngine.Editor.Internal.Plugins.WorkerServicePlugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins.WorkerServicePlugin;

internal sealed class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<IWorkerService, WorkerService>();
    }
}
