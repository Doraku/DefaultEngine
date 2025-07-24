using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace DefaultApplication.DockingLayout.Internal.Controls;

internal sealed partial class LayoutDropControl : Grid
{
    public LayoutDropControl()
    {
        InitializeComponent();
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
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

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(LayoutOperation.Id) ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        LayoutTarget.IsVisible = false;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (sender is not Layoutable control
            || e.Data.Get(LayoutOperation.Id) is not LayoutOperation operation)
        {
            return;
        }

        string className = (control.HorizontalAlignment, control.VerticalAlignment) switch
        {
            (HorizontalAlignment.Left, _) => "Left",
            (HorizontalAlignment.Right, _) => "Right",
            (_, VerticalAlignment.Top) => "Top",
            _ => "Bottom",
        };

        if (Parent?.GetLogicalChildren().OfType<HiddenToolsControl>().FirstOrDefault(target => target.Classes.Contains(className)) is not HiddenToolsControl target)
        {
            return;
        }

        operation.RemoveAction(operation.Content);
        target.Tools.Add(operation.Content);
    }
}