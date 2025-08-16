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
using Avalonia.Styling;
using Avalonia.VisualTree;
using Avalonia.DefaultLayout.Controls;

namespace Avalonia.DefaultLayout.Internal.Controls;

internal sealed partial class HideablesControl : Panel
{
    public static readonly StyledProperty<IList<ILayoutContent>?> ItemsSourceProperty = AvaloniaProperty.Register<LayoutControl, IList<ILayoutContent>?>(nameof(ItemsSource), default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public IList<ILayoutContent>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private readonly ConditionalWeakTable<ILayoutContent, Flyout> _flyouts;

    public HideablesControl()
    {
        _flyouts = [];

        ItemsSource = new ObservableCollection<ILayoutContent>
        {
             new LayoutContent(LayoutOptions.Closable | LayoutOptions.Movable | LayoutOptions.Hideable | LayoutOptions.Stackable, Guid.NewGuid().ToString()),
             new LayoutContent(LayoutOptions.Closable | LayoutOptions.Movable | LayoutOptions.Hideable, Guid.NewGuid().ToString())
        };

        InitializeComponent();
    }

    private void OnHideableClicked(object? sender, PointerPressedEventArgs e)
    {
        Flyout Create(ILayoutContent content)
        {
            Flyout flyout = new()
            {
                Content = content,
                OverlayDismissEventPassThrough = true,
                Placement =
                    Classes.Contains("Top") ? PlacementMode.Bottom
                    : Classes.Contains("Left") ? PlacementMode.Right
                    : Classes.Contains("Right") ? PlacementMode.Left
                    : PlacementMode.Top,
                FlyoutPresenterTheme = this.TryFindResource("HideableFlyout", ActualThemeVariant, out object? resource) && resource is ControlTheme theme ? theme : null
            };

            flyout.FlyoutPresenterClasses.AddRange(Classes.Where(c => !c.StartsWith(':')));
            flyout.FlyoutPresenterClasses.Add("HideableFlyout");

            return flyout;
        }

        if (sender is not Visual control
            || !e.GetCurrentPoint(control).Properties.IsLeftButtonPressed
            || control.DataContext is not ILayoutContent content)
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
            .OfType<HideablesControl>()
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