using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.Plugins;

public interface IServiceRegisterer
{
    void Register(IServiceCollection services);
}
