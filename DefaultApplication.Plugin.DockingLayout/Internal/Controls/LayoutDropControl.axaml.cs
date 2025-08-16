using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using DefaultApplication.DockingLayout.Internal.Views;

namespace DefaultApplication.DockingLayout.Internal.Controls;

internal sealed partial class LayoutDropControl : Grid
{
    public static readonly StyledProperty<bool> AllowStackingProperty = AvaloniaProperty.Register<LayoutDropControl, bool>(nameof(AllowStacking), false);

    public bool AllowStacking
    {
        get => GetValue(AllowStackingProperty);
        set => SetValue(AllowStackingProperty, value);
    }

    public LayoutDropControl()
    {
        InitializeComponent();

        foreach (ContentPresenter target in this.GetVisualDescendants().OfType<ContentPresenter>().Where(border => border.Classes.Contains("LayoutTarget")))
        {
            target.AddHandler(DragDrop.DragEnterEvent, OnTargetDragEnter);
            target.AddHandler(DragDrop.DragLeaveEvent, OnTargetDragLeave);
            target.AddHandler(DragDrop.DragOverEvent, OnTargetDragOver);
            target.AddHandler(DragDrop.DropEvent, OnTargetDrop);
        }
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data.Get(LayoutOperation.Id) is not LayoutOperation operation)
        {
            return;
        }

        if (operation.Content.Options.HasFlag(LayoutOptions.Stackable))
        {
        }
    }

    private void OnTargetDragEnter(object? sender, DragEventArgs e)
    {
        if (sender is not Layoutable control)
        {
            return;
        }

        LayoutTarget.HorizontalAlignment = control.HorizontalAlignment switch
        {
            HorizontalAlignment.Center => HorizontalAlignment.Stretch,
            _ => control.HorizontalAlignment
        };

        LayoutTarget.VerticalAlignment = control.VerticalAlignment switch
        {
            VerticalAlignment.Center => VerticalAlignment.Stretch,
            _ => control.VerticalAlignment
        };

        LayoutTarget.Width = LayoutTarget.HorizontalAlignment != HorizontalAlignment.Stretch ? Bounds.Width / 2 : double.NaN;
        LayoutTarget.Height = LayoutTarget.VerticalAlignment != VerticalAlignment.Stretch ? Bounds.Height / 2 : double.NaN;

        LayoutTarget.IsVisible = true;
    }

    private void OnTargetDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(LayoutOperation.Id) ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnTargetDragLeave(object? sender, DragEventArgs e)
    {
        LayoutTarget.IsVisible = false;
    }

    private void OnTargetDrop(object? sender, DragEventArgs e)
    {
        LayoutTarget.IsVisible = false;

        if (sender is not Layoutable control
            || this.FindAncestorOfType<LayoutContentPresenter>() is not LayoutContentPresenter presenter
            || e.Data.Get(LayoutOperation.Id) is not LayoutOperation operation
            || operation.Presenter == presenter)
        {
            return;
        }

        (Orientation? orientation, bool insertFirst) = (control.HorizontalAlignment, control.VerticalAlignment) switch
        {
            (HorizontalAlignment.Center, VerticalAlignment.Center) => (null as Orientation?, false),
            (_, VerticalAlignment.Top) => (Orientation.Vertical, true),
            (_, VerticalAlignment.Bottom) => (Orientation.Vertical, false),
            (HorizontalAlignment.Left, _) => (Orientation.Horizontal, true),
            (HorizontalAlignment.Right, _) => (Orientation.Horizontal, false),
            _ => (default, default)
        };

        if (orientation is null && presenter.Content is { } && !operation.Content.Options.HasFlag(LayoutOptions.Stackable))
        {
            return;
        }

        ILayoutContent newContent;

        if (presenter.Content is null)
        {
            newContent = operation.Content;
        }
        else if (orientation is null)
        {
            if (presenter.Content is not StackedLayoutContent stack)
            {
                stack = [presenter.Content];
            }

            stack.Add(operation.Content);
            newContent = stack;
        }
        else
        {
            newContent = LayoutContentView.AddAsSplit(presenter.Content, operation.Content, orientation.Value, insertFirst);
        }

        if (presenter.Content != newContent)
        {
            presenter.Content = newContent;
        }

        operation.RemoveAction();
    }
}