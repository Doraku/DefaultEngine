using Avalonia.Controls;
using Avalonia.Styling;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Settings;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<EnvironmentGeneralSettings>]
internal sealed partial class EnvironmentGeneralSettingsView : Grid
{
    public static ThemeVariant[] Themes { get; } = [ThemeVariant.Light, ThemeVariant.Dark];

    public EnvironmentGeneralSettingsView()
    {
        InitializeComponent();
    }
}