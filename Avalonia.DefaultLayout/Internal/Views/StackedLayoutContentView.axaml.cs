using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.DefaultLayout.Internal.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.DefaultLayout.Internal.Views;

public sealed partial class StackedLayoutContentView : DockPanel
{
    public StackedLayoutContentView()
    {
        InitializeComponent();
    }

    private Action GetRemoveAction(object? sender)
    {
        if (this.FindLogicalAncestorOfType<LayoutContentPresenter>() is LayoutContentPresenter parent
            && sender is StyledElement control
            && control.DataContext is ILayoutContent content
            && DataContext is StackedLayoutContent stacked)
        {
            return () =>
            {
                if (stacked.Remove(content) && stacked.Count is 1)
                {
                    parent.Content = stacked.FirstOrDefault();
                }
            };
        }

        return static () => { };
    }

    private void OnHeaderClicked(object? sender, PointerPressedEventArgs e)
    {
        void OnDragged(object? sender, PointerEventArgs e)
        {
            if (sender is not InputElement control
                || control.DataContext is not ILayoutContent content
                || this.FindLogicalAncestorOfType<LayoutContentPresenter>() is not LayoutContentPresenter presenter)
            {
                return;
            }

            control.PointerReleased -= OnDropped;
            control.PointerMoved -= OnDragged;

            DataObject data = new();

            data.Set(LayoutOperation.Id, new LayoutOperation(
                presenter,
                content,
                GetRemoveAction(sender)));

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
            || control.DataContext is not ILayoutContent content
            || !content.Options.HasFlag(LayoutOptions.Movable))
        {
            return;
        }

        control.PointerReleased += OnDropped;
        control.PointerMoved += OnDragged;
    }

    private void OnClose(object? sender, RoutedEventArgs e) => GetRemoveAction(sender)();
}