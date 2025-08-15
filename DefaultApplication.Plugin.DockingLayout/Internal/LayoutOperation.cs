using System;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed record LayoutOperation(
    ILayoutContent Content,
    Action RemoveAction)
{
    public const string Id = "DockingLayoutPlugin.LayoutOperation";
}
