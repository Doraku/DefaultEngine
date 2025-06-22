using Avalonia.Controls;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.Views;

[DataTemplate<SettingsViewModel>(ServiceLifetime.Singleton)]
internal sealed partial class SettingsView : DockPanel
{
    public SettingsView()
    {
        InitializeComponent();
    }
}