using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.DockingLayout.Controls;
using DefaultApplication.DockingLayout.Internal.Controls;

namespace DefaultApplication.DockingLayout.Internal.Views;

[DataTemplate<LayoutContent>]
public sealed partial class LayoutContentView : Border
{
    public LayoutContentView()
    {
        InitializeComponent();
    }

    public static ILayoutContent AddAsSplit(ILayoutContent parentContent, ILayoutContent newContent, Orientation orientation, bool insertFirst)
    {
        if (parentContent is not SplitLayoutContent split || split.Orientation != orientation)
        {
            split = new SplitLayoutContent(orientation)
            {
                new SplitLayoutItem(parentContent, GridLength.Star)
            };
        }

        if (insertFirst)
        {
            split.Insert(0, new SplitLayoutItem(newContent, GridLength.Star));
        }
        else
        {
            split.Add(new SplitLayoutItem(newContent, GridLength.Star));
        }

        return split;
    }

    private Action GetRemoveAction()
    {
        if (this.FindLogicalAncestorOfType<LayoutContentPresenter>() is LayoutContentPresenter parent)
        {
            return parent.GetRemoveAction();
        }

        return static () => { };
    }

    private void OnHeaderClicked(object? sender, PointerPressedEventArgs e)
    {
        void OnDragged(object? sender, PointerEventArgs e)
        {
            if (sender is not InputElement control
                || this.FindLogicalAncestorOfType<LayoutContentPresenter>() is not LayoutContentPresenter presenter)
            {
                return;
            }

            control.PointerReleased -= OnDropped;
            control.PointerMoved -= OnDragged;

            DataObject data = new();

            data.Set(LayoutOperation.Id, new LayoutOperation(
                presenter,
                (ILayoutContent)DataContext,
                GetRemoveAction()));

            DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
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

        if (sender is not InputElement control
            || DataContext is not ILayoutContent content
            || !content.Options.HasFlag(LayoutOptions.Movable))
        {
            return;
        }

        control.PointerReleased += OnDropped;
        control.PointerMoved += OnDragged;
    }

    private void OnClose(object? sender, RoutedEventArgs e) => GetRemoveAction()();

    private void OnToggleHide(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button control
            || DataContext is not ILayoutContent content
            || this.FindLogicalAncestorOfType<LayoutControl>() is not LayoutControl root)
        {
            return;
        }

        if (control.FindLogicalAncestorOfType<FlyoutPresenter>() is { })
        {
            if (this.FindLogicalAncestorOfType<HideablesControl>() is not HideablesControl parent
                || !(parent.ItemsSource?.Remove(content) ?? false))
            {
                return;
            }

            FlyoutBase.GetAttachedFlyout(parent)?.Hide();

            if (root.Content is null)
            {
                root.Content = content;

                return;
            }

            (Orientation orientation, bool insertFirst) = (Grid.GetColumn(parent), Grid.GetRow(parent)) switch
            {
                (1, 0) => (Orientation.Vertical, true),
                (0, 1) => (Orientation.Horizontal, true),
                (1, 2) => (Orientation.Vertical, false),
                (2, 1) => (Orientation.Horizontal, false),
                _ => (Orientation.Vertical, false)
            };

            ILayoutContent newContent = AddAsSplit(root.Content, content, orientation, insertFirst);

            if (root.Content != newContent)
            {
                root.Content = newContent;
            }
        }
        else
        {
            string hideablesClass = "Bottom";
            if (this.GetLogicalAncestors().OfType<LayoutContentPresenter>().Skip(1).FirstOrDefault() is LayoutContentPresenter parent
                && parent.Content is SplitLayoutContent split)
            {
                hideablesClass = (split.Orientation, split.FirstOrDefault()?.Content == content) switch
                {
                    (Orientation.Horizontal, true) => "Left",
                    (Orientation.Horizontal, false) => "Right",
                    (Orientation.Vertical, true) => "Top",
                    (Orientation.Vertical, false) => "Bottom",
                    _ => "Bottom"
                };
            }

            if (root.GetVisualDescendants().OfType<HideablesControl>().FirstOrDefault(hideables => hideables.Classes.Contains(hideablesClass)) is HideablesControl hideables)
            {
                OnClose(sender, e);
                hideables.ItemsSource?.Add(content);
            }
        }
    }
}