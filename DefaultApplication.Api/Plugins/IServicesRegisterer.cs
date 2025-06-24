using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.Plugins;

public interface IServicesRegisterer
{
    void Register(IServiceCollection services);
}
