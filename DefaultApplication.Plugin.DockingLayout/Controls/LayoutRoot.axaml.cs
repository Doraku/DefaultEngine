using Avalonia.Controls;
using Avalonia.Input;

namespace DefaultApplication.DockingLayout.Controls;

public sealed partial class LayoutRoot : Grid
{
    public LayoutRoot()
    {
        InitializeComponent();
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        Classes.Add("LayoutOver");
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        Classes.Remove("LayoutOver");
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        Classes.Remove("LayoutOver");
    }
}