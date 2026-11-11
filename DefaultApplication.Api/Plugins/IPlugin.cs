using System.Threading;
using System.Threading.Tasks;

namespace DefaultApplication.Plugins;

public interface IPlugin
{
    Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
