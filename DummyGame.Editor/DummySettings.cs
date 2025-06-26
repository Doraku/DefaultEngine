using System.Collections.Generic;
using DefaultApplication;

namespace DummyGame.Editor;

internal sealed class DummySettings : ISettings
{
    public IReadOnlyList<string> Path { get; } = ["Environment", "Dummy"];

    public string? Value1 { get; set; }

    public bool Value2 { get; set; }

    public string? Value3 { get; set; }

    public string? Value4 { get; set; }
}
