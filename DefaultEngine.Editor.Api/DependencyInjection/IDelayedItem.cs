using System.Threading.Tasks;

namespace DefaultEngine.Editor.Api.DependencyInjection;

public interface IDelayedItem<T>
{
    Task<T> Value { get; }
}
