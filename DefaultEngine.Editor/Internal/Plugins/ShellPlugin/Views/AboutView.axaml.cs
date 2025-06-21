using Avalonia.Controls;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<AboutViewModel>]
internal sealed partial class AboutView : DockPanel
{
    public AboutView()
    {
        InitializeComponent();
    }
}