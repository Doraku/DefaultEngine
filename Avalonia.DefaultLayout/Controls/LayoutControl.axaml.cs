using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.DefaultLayout.Internal;
using Avalonia.DefaultLayout.Internal.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Avalonia.DefaultLayout.Controls;

public sealed class LayoutControl : TemplatedControl
{
    public static readonly StyledProperty<ILayoutContent?> ContentProperty = AvaloniaProperty.Register<LayoutControl, ILayoutContent?>(nameof(Content), default);

    public ILayoutContent? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<IList<ILayoutContent>?> LeftHideablesProperty = AvaloniaProperty.Register<LayoutControl, IList<ILayoutContent>?>(nameof(LeftHideables), default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public IList<ILayoutContent>? LeftHideables
    {
        get => GetValue(LeftHideablesProperty);
        set => SetValue(LeftHideablesProperty, value);
    }

    public static readonly StyledProperty<IList<ILayoutContent>?> RightHideablesProperty = AvaloniaProperty.Register<LayoutControl, IList<ILayoutContent>?>(nameof(RightHideables), default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public IList<ILayoutContent>? RightHideables
    {
        get => GetValue(RightHideablesProperty);
        set => SetValue(RightHideablesProperty, value);
    }
    public static readonly StyledProperty<IList<ILayoutContent>?> TopHideablesProperty = AvaloniaProperty.Register<LayoutControl, IList<ILayoutContent>?>(nameof(TopHideables), default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public IList<ILayoutContent>? TopHideables
    {
        get => GetValue(TopHideablesProperty);
        set => SetValue(TopHideablesProperty, value);
    }
    public static readonly StyledProperty<IList<ILayoutContent>?> BottomHideablesProperty = AvaloniaProperty.Register<LayoutControl, IList<ILayoutContent>?>(nameof(BottomHideables), default);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public IList<ILayoutContent>? BottomHideables
    {
        get => GetValue(BottomHideablesProperty);
        set => SetValue(BottomHideablesProperty, value);
    }

    public LayoutControl()
    {
        AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDragLeave);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data.Get(LayoutOperation.Id) is not LayoutOperation operation)
        {
            return;
        }

        Classes.Add("LayoutOver");

        foreach (LayoutDropControl control in this.GetVisualDescendants().OfType<LayoutDropControl>())
        {
            control.AllowStacking = operation.Content.Options.HasFlag(LayoutOptions.Stackable);
        }
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        Classes.Remove("LayoutOver");
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
    }
}