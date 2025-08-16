using System;
using Avalonia.DefaultLayout.Internal.Controls;

namespace Avalonia.DefaultLayout.Internal;

internal sealed record LayoutOperation(
    LayoutContentPresenter Presenter,
    ILayoutContent Content,
    Action RemoveAction)
{
    public const string Id = "Avalonia.DefaultLayout.LayoutOperation";
}
