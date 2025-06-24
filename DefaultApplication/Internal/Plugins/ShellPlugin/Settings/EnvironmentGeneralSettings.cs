using System.Collections.Generic;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using DefaultApplication;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Settings;

internal sealed class EnvironmentGeneralSettings : ObservableObject, ISettings
{
    private readonly Application _application;

    public IReadOnlyList<string> Path { get; } = ["Environment", "General"];

    public EnvironmentGeneralSettings(Application application)
    {
        _application = application;
    }

    public ThemeVariant Theme
    {
        get => _application.ActualThemeVariant;
        set => SetProperty(_application.RequestedThemeVariant, value, newValue => _application.RequestedThemeVariant = newValue);
    }
}
