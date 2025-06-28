using System.Threading.Tasks;

namespace DefaultApplication.Plugins;

public interface IPlugin
{
    Task StartAsync() => Task.CompletedTask;
}
