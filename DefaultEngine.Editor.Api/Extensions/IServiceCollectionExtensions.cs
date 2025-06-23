using System.Threading.Tasks;
using DefaultEngine.Editor.Api.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    private sealed record TaskCompletionSourceDelayedItem<T>(Task<T> Value) : IDelayedItem<T>;

    public static IServiceCollection AddDelayedSingleton<T>(this IServiceCollection services, out TaskCompletionSource<T> delayedItem)
    {
        delayedItem = new TaskCompletionSource<T>();
        services.AddSingleton<IDelayedItem<T>>(new TaskCompletionSourceDelayedItem<T>(delayedItem.Task));
        return services;
    }
}
