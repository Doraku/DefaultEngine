using Avalonia;
using DefaultApplication;

namespace DummyGame.Editor;

internal sealed class Program
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<Application>()
            .UsePlatformDetect();

    private static void Main(string[] args)
    {
        using DesktopRuner runner = new();

        runner.Run(args);
    }
}
