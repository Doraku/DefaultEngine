using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using DefaultApplication.ComponentModel;
using DefaultApplication.Settings;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Settings;

internal sealed class EnvironmentGeneralSettings : BaseJsonSettings
{
    private readonly record struct Save(string? Theme);

    private const string _filePath = "EnvironmentGeneralSettings.json";

    public static ThemeVariant[] Themes { get; } = [ThemeVariant.Light, ThemeVariant.Dark];

    private readonly Application? _application;

    private ThemeVariant _theme;

    public EnvironmentGeneralSettings(ILogger<EnvironmentGeneralSettings> logger, Application? application = null)
        : base(logger, "Environment", "General")
    {
        _application = application;

        _theme = ThemeVariant.Light;
    }

    [SettingsInformation("Color theme", "change the color theme of the whole application", nameof(Themes))]
    public ThemeVariant Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            if (_application is { })
            {
                Dispatcher.UIThread.Invoke(() => _application.RequestedThemeVariant = value);
            }
        }
    }

    public override void Read()
    {
        Save? save = Deserialize<Save?>(Logger, _filePath);

        Theme = (save?.Theme ?? "Light") switch
        {
            "Dark" => ThemeVariant.Dark,
            _ => ThemeVariant.Light,
        };
    }

    public override void Write() => Serialize(Logger, _filePath, new Save(Theme == ThemeVariant.Dark ? "Dark" : "Light"));
}
