using System.Collections.Generic;
using System.ComponentModel;
using DefaultApplication.ComponentModel;
using DefaultApplication.Settings;

namespace DummyGame.Editor;

internal sealed class Dummy2Settings : ISettings
{
    public IReadOnlyList<string> Path { get; } = ["Environment", "Dummy"];

    public static IEnumerable<string> Pouet { get; } = ["kikoo", "lol"];

    [Description("this is a long name")]
    public string? Value1 { get; set; }

    public bool ValueBool { get; set; }

    [SettingsItemsSource(nameof(Path))]
    public string? Value3 { get; set; }

    [SettingsItemsSource(nameof(Pouet))]
    public string? Value4 { get; set; }

    public decimal ValueDecimal { get; set; }

    public int ValueInt { get; set; }

    public byte ValueByte { get; set; }

    public float ValueFloat { get; set; }

    public void Read() { }
    public void Write() { }
}
