using Avalonia.Controls;

namespace DefaultApplication.DockingLayout.Internal.Controls;

internal sealed partial class DockableControl : Border
{
    public object? DockableContent
    {
        get => Presenter.Content;
        set => Presenter.Content = value;
    }

    public DockableControl()
    {
        InitializeComponent();
    }
}