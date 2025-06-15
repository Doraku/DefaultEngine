using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor.Api.Plugins;

public interface IServicesRegisterer
{
    void Register(IServiceCollection services);
}
