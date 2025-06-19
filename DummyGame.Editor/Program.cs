using Avalonia;
using DefaultEngine.Editor;

namespace DummyGame.Editor;

internal sealed class Program
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<Application>()
            .UsePlatformDetect();

    private static void Main(string[] args)
    {
        using Runer runner = new();

        runner.Run(args);
    }
}
