using DefaultApplication.Plugins;
using DefaultApplication.Services;
using DefaultApplication.Internal.Plugins.WorkerServicePlugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.WorkerServicePlugin;

internal sealed class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<IWorkerService, WorkerService>();
    }
}
