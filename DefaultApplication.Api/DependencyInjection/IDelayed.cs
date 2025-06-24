using System.Threading.Tasks;

namespace DefaultApplication.DependencyInjection;

public interface IDelayed<T>
{
    Task<T> Task { get; }
}
