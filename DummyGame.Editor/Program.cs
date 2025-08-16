using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultApplication;
using DefaultApplication.DefaultLayout;

namespace DummyGame.Editor;

internal sealed class Pouet : ICommandMenu
{
    private readonly IDockingLayoutService _service;

    public Pouet(IDockingLayoutService service)
    {
        _service = service;
    }

    public IReadOnlyList<string> Path { get; } = ["Test", "pouet"];

    public void Execute()
    { }
}

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        using DesktopRuner runner = new();

        await runner.RunAsync(args).ConfigureAwait(false);
    }
}