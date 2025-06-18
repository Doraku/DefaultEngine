using Avalonia;
using DefaultEngine.Editor;

namespace DummyGame.Editor;

internal sealed class Program
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<DefaultEditor>()
            .UsePlatformDetect();

    private static void Main(string[] args) => DefaultEditor.Run<DefaultEditor>(args);
}
