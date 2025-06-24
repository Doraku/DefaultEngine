using Avalonia.Controls;
using Avalonia.Styling;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Internal.Plugins.ShellPlugin.Settings;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<EnvironmentGeneralSettings>]
internal sealed partial class EnvironmentGeneralSettingsView : Grid
{
    public static ThemeVariant[] Themes { get; } = [ThemeVariant.Light, ThemeVariant.Dark];

    public EnvironmentGeneralSettingsView()
    {
        InitializeComponent();
    }
}