using System;

namespace DefaultEngine.Editor.Api;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class EditorMenuAttribute : Attribute
{
    public string DisplayName { get; }

    public EditorMenuAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}
