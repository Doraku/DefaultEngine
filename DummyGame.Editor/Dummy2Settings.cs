using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DefaultApplication.ComponentModel;
using DefaultApplication.Settings;

namespace DummyGame.Editor;

internal sealed class Dummy2Settings : ISettings
{
    public IReadOnlyList<string> Path { get; } = ["Environment", "Dummy"];

    public static IEnumerable<string> Pouet { get; } = ["kikoo", "lol"];

    [SettingsInformation("this is a long name")]
    public string? Value1 { get; set; }

    public bool ValueBool { get; set; }

    [SettingsInformation(itemsSourceMember: nameof(Path))]
    public string? Value3 { get; set; }

    [SettingsInformation(itemsSourceMember: nameof(Pouet))]
    public string? Value4 { get; set; }

    public decimal ValueDecimal { get; set; }

    public int ValueInt { get; set; }

    public byte ValueByte { get; set; }

    public float ValueFloat { get; set; }

    public Task ReadAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task WriteAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
