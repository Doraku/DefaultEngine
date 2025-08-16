using System;
using DefaultApplication.DockingLayout.Internal.Controls;

namespace DefaultApplication.DockingLayout.Internal;

internal sealed record LayoutOperation(
    LayoutContentPresenter Presenter,
    ILayoutContent Content,
    Action RemoveAction)
{
    public const string Id = "DockingLayoutPlugin.LayoutOperation";
}
