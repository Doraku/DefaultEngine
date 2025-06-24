using Avalonia.Controls;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Internal.Plugins.AboutPlugin.ViewModels;

namespace DefaultApplication.Internal.Plugins.AboutPlugin.Views;

[DataTemplate<AboutViewModel>]
internal sealed partial class AboutView : DockPanel
{
    public AboutView()
    {
        InitializeComponent();
    }
}