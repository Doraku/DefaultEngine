using System;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using DefaultApplication.DockingLayout.Controls;

namespace DefaultApplication.DockingLayout.Internal.Controls;

public class LayoutContentPresenter : TemplatedControl
{
    private Action GetRemoveAction()
    {
        if (DataContext is { })
        {
            if (this.FindLogicalAncestorOfType<HideableItemsControl>() is HideableItemsControl HideableItemsControl)
            {
                FlyoutBase.GetAttachedFlyout(HideableItemsControl)?.Hide();

                return () => HideableItemsControl.Hideables.Remove(DataContext);
            }
            else if (Parent?.Parent is SplittedGrid grid
                && grid.Parent is ContentPresenter parent)
            {
                ContentPresenter remainingPresenter = grid.FirstContent.Content == this ? grid.SecondContent : grid.FirstContent;
                object? remainingContent = remainingPresenter.Content;

                return () =>
                {
                    remainingPresenter.Content = null;
                    parent.Content = remainingContent;
                };
            }
            else if (this.FindLogicalAncestorOfType<LayoutRoot>() is LayoutRoot root)
            {
                return () => root.MainContent.Content = null;
            }
        }

        return static () => { };
    }

    private void OnHeaderClicked(object? sender, PointerPressedEventArgs e)
    {
        void OnDragged(object? sender, PointerEventArgs e)
        {
            if (sender is not InputElement control)
            {
                return;
            }

            control.PointerReleased -= OnDropped;
            control.PointerMoved -= OnDragged;

            DataObject data = new();

            data.Set(LayoutOperation.Id, new LayoutOperation(control.DataContext!, GetRemoveAction()));

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
            || control.DataContext is not object content
            || !content.IsMovable())
        {
            return;
        }

        control.PointerReleased += OnDropped;
        control.PointerMoved += OnDragged;
    }

    private void OnClose(object? sender, RoutedEventArgs e) => GetRemoveAction()();

    private void OnToggleHide(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton control
            || control.DataContext is not object content
            || this.FindLogicalAncestorOfType<LayoutRoot>() is not LayoutRoot root)
        {
            return;
        }

        if (control.IsChecked is false)
        {
            //    if (this.FindLogicalAncestorOfType<HideableItemsControl>() is not HideableItemsControl tools)
            //    {
            //        return;
            //    }

            //    FlyoutBase.GetAttachedFlyout(tools)?.Hide();
            //    tools.Tools.Remove(content);

            //    ToolPresenter newPresenter = new() { DataContext = content };
            //    root.MainContent.Content =
            //        root.MainContent.Content is { }
            //        ? (Grid.GetColumn(tools), Grid.GetRow(tools)) switch
            //        {
            //            (1, 0) => SplittedGrid.CreateVertical(newPresenter, root.MainContent.Content, SplittedGridMode.SecondFill),
            //            (1, 2) => SplittedGrid.CreateVertical(root.MainContent.Content, newPresenter, SplittedGridMode.FirstFill),
            //            (0, 1) => SplittedGrid.CreateHorizontal(newPresenter, root.MainContent.Content, SplittedGridMode.SecondFill),
            //            (2, 1) => SplittedGrid.CreateHorizontal(root.MainContent.Content, newPresenter, SplittedGridMode.FirstFill),
            //            _ => newPresenter
            //        }
            //        : newPresenter;
        }
        else
        {
            //    if (Parent?.Parent is not SplittedGrid grid
            //        || grid.Parent is not ContentPresenter parent)
            //    {
            //        return;
            //    }

            //    string toolsClass =
            //        grid.RowDefinitions.Count > 0
            //        ? (grid.FirstContent.Content == this ? "Top" : "Bottom")
            //        : (grid.FirstContent.Content == this ? "Left" : "Right");

            //    if (root.GetLogicalChildren().OfType<HideableItemsControl>().FirstOrDefault(tools => tools.Classes.Contains(toolsClass)) is HideableItemsControl tools)
            //    {
            //        ContentPresenter remaining = grid.FirstContent.Content == this ? grid.SecondContent : grid.FirstContent;
            //        object? remainingContent = remaining.Content;
            //        remaining.Content = null;
            //        parent.Content = remainingContent;
            //        tools.Tools.Add(content);
            //    }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        void SetupElement<T>(string name, Action<T> setupAction)
            where T : class
        {
            if (e.NameScope.Find<T>(name) is T control)
            {
                setupAction(control);
            }
        }

        ArgumentNullException.ThrowIfNull(e);

        base.OnApplyTemplate(e);

        SetupElement<InputElement>("PART_Header", control => control.PointerPressed += OnHeaderClicked);
        SetupElement<Button>("PART_CloseButton", control => control.Click += OnClose);
        SetupElement<Button>("PART_HideButton", control => control.Click += OnToggleHide);
    }
}