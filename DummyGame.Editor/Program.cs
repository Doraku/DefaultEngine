using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using DefaultApplication;
using DefaultApplication.DefaultLayout;
using DefaultApplication.Services;

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

internal sealed class Pouet2 : IAsyncCommandMenu
{
    private readonly IWorkerService _service;
    private readonly INotificationService _notification;

    public Pouet2(IWorkerService service, INotificationService notification)
    {
        _service = service;
        _notification = notification;
    }

    public IReadOnlyList<string> Path { get; } = ["Test", "pouet2"];

    public Task ExecuteAsync() => _service.ExecuteAsync(async operation =>
    {
        operation.Header = "kikoo";

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        await _notification.ShowInformationAsync("kikoo").ConfigureAwait(false);

        operation.Header = "lol";

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        operation.MaximumProgress = 10;

        for (int i = 0; i < 10; i++)
        {
            await _notification.ShowWarningAsync(i).ConfigureAwait(false);
            operation.Content = $"doing {i}";
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            if (i % 2 == 0)
            {
                await _service.ExecuteAsync(() => Task.Delay(TimeSpan.FromSeconds(2))).ConfigureAwait(false);
                operation.HasError = true;
            }

            operation.CancellationToken.ThrowIfCancellationRequested();

            ++operation.CurrentProgress;
        }
    });
}

internal sealed class Program
{
#if DEBUG
    private sealed class DebugRunner : DesktopRunner
    {
        protected override Task<AppBuilder> ConfigureBuilderAsync(AppBuilder builder)
            => base.ConfigureBuilderAsync(builder.WithDeveloperTools());
    }
#endif

    private static async Task Main(string[] args)
    {
        using DesktopRunner runner =
#if DEBUG
            new DebugRunner();
#else
            new();
#endif

        await runner.RunAsync(args).ConfigureAwait(false);
    }
}