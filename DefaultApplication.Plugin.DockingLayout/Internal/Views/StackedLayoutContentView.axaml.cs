using Avalonia.Controls;
using DefaultApplication.Controls.Metadata;

namespace DefaultApplication.DockingLayout.Internal.Views;

[DataTemplate<StackedLayoutContent>]
public sealed partial class StackedLayoutContentView : UserControl
{
    public StackedLayoutContentView()
    {
        InitializeComponent();
    }
}