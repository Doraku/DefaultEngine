using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

internal static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAsSingletonImplementation<T>(this IServiceCollection services, Type type)
    {
        services.TryAddSingleton(type);
        return services.AddSingleton(typeof(T), provider => provider.GetRequiredService(type));
    }

    public static IServiceCollection AddAsSingletonImplementation<T>(this IServiceCollection services, T instance)
    {
        Type actualType = instance!.GetType();
        services.TryAddSingleton(actualType, _ => instance);
        return services.AddSingleton(typeof(T), provider => provider.GetRequiredService(actualType));
    }
}
