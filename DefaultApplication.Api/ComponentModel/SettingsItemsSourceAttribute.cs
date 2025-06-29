using System;

namespace DefaultApplication.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SettingsItemsSourceAttribute : Attribute
{
    public string MemberName { get; }

    public SettingsItemsSourceAttribute(string memberName)
    {
        MemberName = memberName;
    }
}
