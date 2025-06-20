using Avalonia.Controls;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;

namespace DefaultEngine.Editor;

[DataTemplate<AboutViewModel>]
internal sealed partial class AboutView : StackPanel
{
    public AboutView()
    {
        InitializeComponent();
    }
}