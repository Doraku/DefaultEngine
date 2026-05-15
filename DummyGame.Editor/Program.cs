using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.DefaultLayout;
using DefaultApplication;
using DefaultApplication.DefaultLayout;
using DefaultApplication.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DummyGame.Editor;

internal sealed class GameTest : IAsyncCommandMenu
{
    private sealed class DummyGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public DummyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Your game logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Your drawing code here

            base.Draw(gameTime);
        }
    }

    private readonly IDockingLayoutService _service;

    public GameTest(IDockingLayoutService service)
    {
        _service = service;
    }

    public IReadOnlyList<string> Path { get; } = ["Test", "pouet"];

    public Task ExecuteAsync()
    {
        return _service.ShowAsync(LayoutOptions.Closable | LayoutOptions.Movable | LayoutOptions.Stackable, new DummyGame());
    }
}

internal sealed class NotificationTest : IAsyncCommandMenu
{
    private readonly IWorkerService _service;
    private readonly INotificationService _notification;

    public NotificationTest(IWorkerService service, INotificationService notification)
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