using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.DefaultLayout.Internal.Views;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace Avalonia.DefaultLayout.Internal.Controls;

internal sealed partial class LayoutDropControl : Panel
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
            || (operation.Presenter == presenter && presenter.Content is not StackedLayoutContent))
        {
            return;
        }

        (Orientation? orientation, int insertIndex) = (control.HorizontalAlignment, control.VerticalAlignment) switch
        {
            (HorizontalAlignment.Center, VerticalAlignment.Center) => (null as Orientation?, -1),
            (_, VerticalAlignment.Top) => (Orientation.Vertical, 0),
            (_, VerticalAlignment.Bottom) => (Orientation.Vertical, -1),
            (HorizontalAlignment.Left, _) => (Orientation.Horizontal, 0),
            (HorizontalAlignment.Right, _) => (Orientation.Horizontal, -1),
            _ => (default, -1)
        };

        if (orientation is null && presenter.Content is { } && !operation.Content.Options.HasFlag(LayoutOptions.Stackable))
        {
            return;
        }

        LayoutContentPresenter? parentPresenter = presenter.FindAncestorOfType<LayoutContentPresenter>();

        operation.RemoveAction();

        // in case the remove action remove the presenter from the layout look for the new presenter of current content
        if (presenter.Parent is null && parentPresenter != null)
        {
            presenter = parentPresenter.GetSelfAndVisualDescendants().OfType<LayoutContentPresenter>().FirstOrDefault(child => child.Content == presenter.Content) ?? presenter;
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
            if (presenter.Content is not SplitLayoutContent
                && presenter.FindAncestorOfType<LayoutContentPresenter>() is LayoutContentPresenter parent
                && parent.Content is SplitLayoutContent split
                && split.Orientation == orientation)
            {
                insertIndex = split.Select((item, index) => (Index: index, item.Content)).FirstOrDefault(item => item.Content == presenter.Content).Index + (insertIndex < 0 ? 1 : 0);
                presenter = parent;
            }

            newContent = LayoutContentView.AddAsSplit(presenter.Content, operation.Content, orientation.Value, insertIndex);
        }

        if (presenter.Content != newContent)
        {
            presenter.Content = newContent;
        }
    }
}