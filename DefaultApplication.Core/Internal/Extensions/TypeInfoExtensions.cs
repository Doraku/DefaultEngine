using System.Collections.Generic;
using System.Linq;

namespace System.Reflection;

internal static class TypeInfoExtensions
{
    public static IEnumerable<TypeInfo> GetInstanciableImplementation<T>(this IEnumerable<TypeInfo> types)
        => types.Where(type => !type.IsInterface && !type.IsAbstract && type.IsAssignableTo<T>() && type.GetConstructors().Length != 0);

    public static IEnumerable<(TypeInfo, TAttribute)> GetTypesWithAttribute<TAttribute>(this IEnumerable<TypeInfo> types)
        where TAttribute : Attribute
        => types.SelectMany(type => type.GetCustomAttributes<TAttribute>().Select(attribute => (type, attribute)));
}
