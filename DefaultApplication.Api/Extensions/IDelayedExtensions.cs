using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DefaultApplication.DependencyInjection;

public static class IDelayedExtensions
{
    public static TaskAwaiter<T> GetAwaiter<T>(this IDelayed<T> delayedValue)
    {
        ArgumentNullException.ThrowIfNull(delayedValue);

        return delayedValue.Task.GetAwaiter();
    }

    public static ConfiguredTaskAwaitable<T> ConfigureAwait<T>(this IDelayed<T> delayedValue, bool continueOnCapturedContext)
    {
        ArgumentNullException.ThrowIfNull(delayedValue);

        return delayedValue.Task.ConfigureAwait(continueOnCapturedContext);
    }

    public static ConfiguredTaskAwaitable<T> ConfigureAwait<T>(this IDelayed<T> delayedValue, ConfigureAwaitOptions options)
    {
        ArgumentNullException.ThrowIfNull(delayedValue);

        return delayedValue.Task.ConfigureAwait(options);
    }
}
