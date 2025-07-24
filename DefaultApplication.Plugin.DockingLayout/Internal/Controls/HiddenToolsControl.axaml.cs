using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace DefaultApplication.DockingLayout.Internal.Controls;

internal sealed partial class HiddenToolsControl : ItemsControl
{
    private readonly ConditionalWeakTable<object, Flyout> _flyouts;

    public IList<object> Tools { get; }

    protected override Type StyleKeyOverride => typeof(ItemsControl);

    public HiddenToolsControl()
    {
        _flyouts = [];

        Tools = new ObservableCollection<object>
        {
            "item 1",
            "item 2"
        };

        InitializeComponent();
    }

    private void OnHiddenToolClicked(object? sender, PointerPressedEventArgs e)
    {
        Flyout Create(object content)
        {
            Flyout flyout = new()
            {
                Content = content,
                OverlayDismissEventPassThrough = true,
                Placement =
                    Classes.Contains("Top") ? PlacementMode.Bottom
                    : Classes.Contains("Left") ? PlacementMode.Right
                    : Classes.Contains("Right") ? PlacementMode.Left
                    : PlacementMode.Top
            };

            flyout.FlyoutPresenterClasses.Add("ToolFlyout");

            return flyout;
        }

        if (sender is not StyledElement control
            || control.DataContext is not object content)
        {
            return;
        }

        Flyout flyout = _flyouts.GetValue(content, Create);

        if (flyout.IsOpen)
        {
            flyout.Hide();

            return;
        }

        foreach (FlyoutBase openedFlyout in Parent
            ?.GetLogicalDescendants()
            .OfType<HiddenToolsControl>()
            .Select(FlyoutBase.GetAttachedFlyout)
            .Where(flyout => flyout?.IsOpen ?? false)
            .Select(flyout => flyout!) ?? [])
        {
            openedFlyout.Hide();
        }

        FlyoutBase.SetAttachedFlyout(this, flyout);
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

    private void OnToolFlyoutHeaderBarClicked(object? sender, PointerPressedEventArgs e)
    {
        void Remove(object content)
        {
            _flyouts.Remove(content);
            Tools.Remove(content);
        }

        void OnDragged(object? sender, PointerEventArgs e)
        {
            if (sender is not InputElement control)
            {
                return;
            }

            control.PointerReleased -= OnDropped;
            control.PointerMoved -= OnDragged;

            DataObject data = new();

            data.Set(LayoutOperation.Id, new LayoutOperation(control.DataContext!, this, Remove));

            DragDrop.DoDragDrop(e, data, DragDropEffects.Move);

            FlyoutBase.GetAttachedFlyout(this)?.Hide();
        }

        void OnDropped(object? sender, PointerReleasedEventArgs e)
        {
            if (sender is not InputElement control)
            {
                return;
            }

            control.PointerReleased -= OnDropped;
            control.PointerMoved -= OnDragged;
        }

        if (sender is not InputElement control)
        {
            return;
        }

        control.PointerReleased += OnDropped;
        control.PointerMoved += OnDragged;
    }
}