using Avalonia.Controls;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin.Views;

[DataTemplate<SettingsViewModel>]
internal sealed partial class SettingsView : DockPanel
{
    public SettingsView()
    {
        InitializeComponent();
    }
}