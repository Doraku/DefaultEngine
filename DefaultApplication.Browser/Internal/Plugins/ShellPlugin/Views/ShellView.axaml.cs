using Avalonia.Controls;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<ShellViewModel>]
internal sealed partial class ShellView : Border
{
    public ShellView()
    {
        InitializeComponent();
    }
}