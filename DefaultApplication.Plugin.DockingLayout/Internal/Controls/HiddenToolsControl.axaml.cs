using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace DefaultApplication.DockingLayout.Internal.Controls;

internal sealed partial class HiddenToolsControl : ItemsControl
{
    protected override Type StyleKeyOverride => typeof(ItemsControl);

    public HiddenToolsControl()
    {
        InitializeComponent();
    }

    private void OnHiddenToolClicked(object? sender, PointerPressedEventArgs e)
    {
        if (FlyoutBase.GetAttachedFlyout(this) is not Flyout flyout)
        {
            return;
        }

        if (flyout.IsOpen)
        {
            flyout.Hide();

            return;
        }

        flyout.Placement =
            Classes.Contains("Top") ? PlacementMode.Bottom
            : Classes.Contains("Left") ? PlacementMode.Right
            : Classes.Contains("Right") ? PlacementMode.Left
            : PlacementMode.Top;

        FlyoutBase.ShowAttachedFlyout(this);
    }
}