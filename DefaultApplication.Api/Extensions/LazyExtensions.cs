using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DefaultApplication.DependencyInjection;

public static class LazyExtensions
{
    public static TaskAwaiter<T> GetAwaiter<T>(this Lazy<Task<T>> lazy)
    {
        ArgumentNullException.ThrowIfNull(lazy);

        return lazy.Value.GetAwaiter();
    }

    public static ConfiguredTaskAwaitable<T> ConfigureAwait<T>(this Lazy<Task<T>> lazy, bool continueOnCapturedContext)
    {
        ArgumentNullException.ThrowIfNull(lazy);

        return lazy.Value.ConfigureAwait(continueOnCapturedContext);
    }

    public static ConfiguredTaskAwaitable<T> ConfigureAwait<T>(this Lazy<Task<T>> lazy, ConfigureAwaitOptions options)
    {
        ArgumentNullException.ThrowIfNull(lazy);

        return lazy.Value.ConfigureAwait(options);
    }

    public static TaskAwaiter GetAwaiter(this Lazy<Task> lazy)
    {
        ArgumentNullException.ThrowIfNull(lazy);

        return lazy.Value.GetAwaiter();
    }

    public static ConfiguredTaskAwaitable ConfigureAwait(this Lazy<Task> lazy, bool continueOnCapturedContext)
    {
        ArgumentNullException.ThrowIfNull(lazy);

        return lazy.Value.ConfigureAwait(continueOnCapturedContext);
    }

    public static ConfiguredTaskAwaitable ConfigureAwait(this Lazy<Task> lazy, ConfigureAwaitOptions options)
    {
        ArgumentNullException.ThrowIfNull(lazy);

        return lazy.Value.ConfigureAwait(options);
    }
}
