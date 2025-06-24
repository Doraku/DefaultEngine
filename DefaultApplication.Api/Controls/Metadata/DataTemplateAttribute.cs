using System;

namespace DefaultApplication.Controls.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
public sealed class DataTemplateAttribute<T> : DataTemplateAttribute
{
    public DataTemplateAttribute()
        : base(typeof(T))
    { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1813:Avoid unsealed attributes")]
public class DataTemplateAttribute : Attribute
{
    public Type DataType { get; }

    public DataTemplateAttribute(Type dataType)
    {
        DataType = dataType;
    }
}
