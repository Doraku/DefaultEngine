using System;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultApplication.Controls.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
public sealed class DataTemplateAttribute<T> : DataTemplateAttribute
{
    public DataTemplateAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
        : base(typeof(T), lifetime)
    { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1813:Avoid unsealed attributes")]
public class DataTemplateAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }

    public Type DataType { get; }

    public DataTemplateAttribute(Type dataType, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        Lifetime = lifetime;
        DataType = dataType;
    }
}
