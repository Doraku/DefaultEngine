using System.Threading.Tasks;
using DefaultApplication;

namespace DummyGame.Editor;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        using DesktopRuner runner = new();

        await runner.RunAsync(args).ConfigureAwait(false);
    }
}