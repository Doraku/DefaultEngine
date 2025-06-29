using DefaultApplication.Settings;
using Microsoft.Extensions.Logging;

namespace DummyGame.Editor;

internal sealed class DummySettings : BaseJsonSettings
{
    private const string _filePath = "DummySettings.json";

    private readonly record struct Save(
        string? Value1,
        bool Value2,
        string? Value3,
        string? Value4);

    public DummySettings(ILogger<DummySettings> logger)
        : base(logger, "Environment", "Dummy")
    { }

    public string? Value1 { get; set; }

    public bool Value2 { get; set; }

    public string? Value3 { get; set; }

    public string? Value4 { get; set; }

    public override void Read()
    {
        Save? save = Deserialize<Save?>(Logger, _filePath);

        Value1 = save?.Value1 ?? "kikoo";
        Value2 = save?.Value2 ?? true;
        Value3 = save?.Value3 ?? "lol";
        Value4 = save?.Value4 ?? "ohyeah";
    }

    public override void Write() => Serialize(Logger, _filePath, new Save(
        Value1,
        Value2,
        Value3,
        Value4));
}
