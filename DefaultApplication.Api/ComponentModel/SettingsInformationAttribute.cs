using System;

namespace DefaultApplication.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SettingsInformationAttribute : Attribute
{
    public string? Name { get; }

    public string? Description { get; }

    public string? ItemsSourceMember { get; }

    public SettingsInformationAttribute(
        string? name = null,
        string? description = null,
        string? itemsSourceMember = null)
    {
        Name = name;
        Description = description;
        ItemsSourceMember = itemsSourceMember;
    }
}
