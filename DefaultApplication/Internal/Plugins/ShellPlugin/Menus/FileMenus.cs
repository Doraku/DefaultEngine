using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;
using DefaultApplication;
using DefaultApplication.Services;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Menus;

internal sealed class FileMenu : IMenu
{
    public int Order => int.MinValue;

    public IReadOnlyList<string> Path { get; } = ["File"];
}

internal sealed class ExitMenu : ICommandMenu
{
    internal sealed record Message;

    private readonly IMessenger _messenger;

    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["File", "Exit"];

    public KeyGesture HotKey { get; } = new(Key.F4, KeyModifiers.Alt);

    public ExitMenu(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public void Execute() => _messenger.Send<Message>();
}

internal sealed class TestMenu : IAsyncCommandMenu
{
    private readonly IWorkerService _service;

    public IReadOnlyList<string> Path { get; } = ["File", "Test"];

    public TestMenu(IWorkerService service)
    {
        _service = service;
    }

    public Task ExecuteAsync() => _service.ExecuteAsync(async operation =>
    {
        operation.Name = "kikoo";

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        operation.Name = "lol";

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        operation.MaximumProgress = 10;

        for (int i = 0; i < 10; i++)
        {
            operation.Name = $"doing {i}";
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
