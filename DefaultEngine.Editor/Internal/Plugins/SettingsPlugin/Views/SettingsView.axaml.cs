using Avalonia.Controls;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.ViewModels;

namespace DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.Views;

[DataTemplate<SettingsViewModel>]
internal sealed partial class SettingsView : DockPanel
{
    public SettingsView()
    {
        InitializeComponent();
    }
}