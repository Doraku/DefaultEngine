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

internal sealed partial class HideableItemsControl : ItemsControl
{
    private readonly ConditionalWeakTable<object, Flyout> _flyouts;

    public IList<object> Hideables { get; }

    protected override Type StyleKeyOverride => typeof(ItemsControl);

    public HideableItemsControl()
    {
        _flyouts = [];

        Hideables = new ObservableCollection<object>
        {
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        };

        foreach (object item in Hideables)
        {
            item.SetLayoutOptions(LayoutOptions.Closable | LayoutOptions.Hideable | LayoutOptions.Movable);
        }

        InitializeComponent();
    }

    private void OnHideableClicked(object? sender, PointerPressedEventArgs e)
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

            flyout.FlyoutPresenterClasses.Add("HideableFlyout");

            return flyout;
        }

        if (sender is not Visual control
            || !e.GetCurrentPoint(control).Properties.IsLeftButtonPressed
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
            .OfType<HideableItemsControl>()
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
}