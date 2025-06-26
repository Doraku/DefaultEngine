using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using DefaultApplication.ComponentModel;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Settings;

internal sealed class EnvironmentGeneralSettings : ObservableObject, ISettings
{
    private readonly Application _application;

    public static ThemeVariant[] Themes { get; } = [ThemeVariant.Light, ThemeVariant.Dark];

    public IReadOnlyList<string> Path { get; } = ["Environment", "General"];

    public EnvironmentGeneralSettings(Application application)
    {
        _application = application;
    }

    [Description("Color theme")]
    [ItemsSource(nameof(Themes))]
    public ThemeVariant Theme
    {
        get => _application.ActualThemeVariant;
        set => SetProperty(_application.RequestedThemeVariant, value, newValue => _application.RequestedThemeVariant = newValue);
    }
}
