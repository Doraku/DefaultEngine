using System;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace Avalonia.DefaultLayout.Internal.Controls;

internal sealed class LayoutContentPresenter : TemplatedControl
{
    public static readonly StyledProperty<ILayoutContent?> ContentProperty = AvaloniaProperty.Register<LayoutContentPresenter, ILayoutContent?>(nameof(Content), default);

    public ILayoutContent? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<bool> IsRootProperty = AvaloniaProperty.Register<LayoutContentPresenter, bool>(nameof(IsRoot), false);

    public bool IsRoot
    {
        get => GetValue(IsRootProperty);
        set => SetValue(IsRootProperty, value);
    }

    public Action GetRemoveAction()
    {
        if (Content is { })
        {
            if (this.FindLogicalAncestorOfType<HideablesControl>() is HideablesControl hideablesControl)
            {
                FlyoutBase.GetAttachedFlyout(hideablesControl)?.Hide();

                return () => hideablesControl.ItemsSource?.Remove(Content);
            }
            else if (this.FindLogicalAncestorOfType<LayoutContentPresenter>() is not LayoutContentPresenter parent)
            {
                return () => Content = null;
            }
            else if (parent.Content is LayoutContent single)
            {
                return parent.GetRemoveAction();
            }
            else if (parent.Content is SplitLayoutContent split && split.FirstOrDefault(item => item.Content == Content) is SplitLayoutItem item)
            {
                return
                    () =>
                    {
                        if (split.Remove(item) && split.Count is 1)
                        {
                            parent.Content = split.FirstOrDefault()?.Content;
                        }
                    };
            }
            else if (parent.Content is StackedLayoutContent stacked)
            {
                return () =>
                {
                    if (stacked.Remove(Content) && stacked.Count is 1)
                    {
                        parent.Content = stacked.FirstOrDefault();
                    }
                };
            }
        }

        return static () => { };
    }
}