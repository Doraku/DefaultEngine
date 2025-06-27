using System.Threading.Tasks;
using Avalonia;
using DefaultApplication;

namespace DummyGame.Editor;

internal sealed class Program
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<Application>()
            .UsePlatformDetect();

    private static async Task Main(string[] args)
    {
        using DesktopRuner runner = new();

        await runner.RunAsync(args).ConfigureAwait(false);
    }
}