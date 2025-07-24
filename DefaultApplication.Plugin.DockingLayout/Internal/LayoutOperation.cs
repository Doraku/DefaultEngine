using System;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed record LayoutOperation(
    object Content,
    object Parent,
    Action<object> RemoveAction)
{
    public const string Id = "DockingLayoutPlugin.LayoutOperation";
}
