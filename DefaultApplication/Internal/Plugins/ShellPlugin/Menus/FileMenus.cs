﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using DefaultApplication.DependencyInjection;
using DefaultApplication.Services;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Menus;

internal sealed class FileMenu : IMenu
{
    public int Order => int.MinValue;

    public IReadOnlyList<string> Path { get; } = ["File"];
}

internal sealed class ExitMenu : IAsyncCommandMenu
{
    private readonly IDelayed<TopLevel> _mainTopLevel;

    public int Order => int.MaxValue;

    public IReadOnlyList<string> Path { get; } = ["File", "Exit"];

    public KeyGesture HotKey { get; } = new(Key.F4, KeyModifiers.Alt);

    public ExitMenu(IDelayed<TopLevel> mainTopLevel)
    {
        _mainTopLevel = mainTopLevel;
    }

    public async Task ExecuteAsync() => ((Window)await _mainTopLevel.Task.ConfigureAwait(true)).Close();
}

internal sealed class TestMenu : IAsyncCommandMenu
{
    private readonly IWorkerService _service;
    private readonly INotificationService _notification;

    public IReadOnlyList<string> Path { get; } = ["File", "Test"];

    public TestMenu(IWorkerService service, INotificationService notification)
    {
        _service = service;
        _notification = notification;
    }

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
