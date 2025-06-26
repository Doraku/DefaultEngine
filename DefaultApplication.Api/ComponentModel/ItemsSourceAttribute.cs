using System;

namespace DefaultApplication.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ItemsSourceAttribute : Attribute
{
    public string MemberName { get; }

    public ItemsSourceAttribute(string memberName)
    {
        MemberName = memberName;
    }
}
