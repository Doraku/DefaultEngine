using System.Threading.Tasks;
using DefaultApplication.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    private sealed record TaskCompletionSourceDelayedItem<T>(Task<T> Task) : IDelayed<T>;

    public static IServiceCollection AddDelayedSingleton<T>(this IServiceCollection services, out TaskCompletionSource<T> delayedValue)
    {
        delayedValue = new TaskCompletionSource<T>();
        services.AddSingleton<IDelayed<T>>(new TaskCompletionSourceDelayedItem<T>(delayedValue.Task));
        return services;
    }
}
