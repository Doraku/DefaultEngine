using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

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

    private void OnFlyoutResize(object? sender, PointerPressedEventArgs e)
    {
        void OnResising(object? sender, PointerEventArgs e)
        {
            if (sender is not Visual control
                || control.FindAncestorOfType<FlyoutPresenter>() is not FlyoutPresenter presenter)
            {
                return;
            }

            Point offset = e.GetPosition(presenter);

            if (Classes.Contains("Vertical"))
            {
                presenter.Width = Classes.Contains("Left") ? Math.Max(presenter.MinWidth, offset.X) : Math.Max(presenter.MinWidth, presenter.Width - offset.X);
            }
            else
            {
                presenter.Height = Classes.Contains("Top") ? Math.Max(presenter.MinHeight, offset.Y) : Math.Max(presenter.MinHeight, presenter.Height - offset.Y);
            }
        }

        void OnResized(object? sender, PointerReleasedEventArgs e)
        {
            if (sender is not InputElement control)
            {
                return;
            }

            control.PointerMoved -= OnResising;
            control.PointerReleased -= OnResized;
        }

        if (sender is not InputElement control)
        {
            return;
        }

        control.PointerMoved += OnResising;
        control.PointerReleased += OnResized;
    }
}