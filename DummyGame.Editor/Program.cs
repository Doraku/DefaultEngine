using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.DefaultLayout;
using DefaultApplication;
using DefaultApplication.DefaultLayout;
using DefaultApplication.Services;
using DefaultEngine.Editor.Internal.Plugins.MonoGamePlugin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DummyGame.Editor;

internal sealed class GameTest : IAsyncCommandMenu
{
    private sealed class DummyGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private ResolutionRenderer? _resolutionRenderer;
        private SpriteBatch? _spriteBatch;

        private Texture2D? _texture;
        private Vector2 _position;
        private Vector2 _velocity;

        public DummyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _position = new Vector2(500, 500);
            _velocity = new Vector2(1000, 500);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _resolutionRenderer = new ResolutionRenderer(new Microsoft.Xna.Framework.Point(1920, 1080), _graphics.GraphicsDevice);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _texture.SetData<byte>([255, 255, 255, 255]);
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Your game logic here
            base.Update(gameTime);

            _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_position.X is >= 1920 or <= 0)
            {
                _velocity *= new Vector2(-1, 1);
            }

            if (_position.Y is >= 1080 or <= 0)
            {
                _velocity *= new Vector2(1, -1);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _resolutionRenderer?.Begin();

            base.Draw(gameTime);

            _spriteBatch?.Begin();

            _spriteBatch?.Draw(_texture, new Rectangle(_position.ToPoint(), new Microsoft.Xna.Framework.Point(100, 100)), Color.Red);

            _spriteBatch?.End();

            _resolutionRenderer?.End();
        }

        protected override void Dispose(bool disposing)
        {
            _resolutionRenderer?.Dispose();
            _graphics.Dispose();
            _spriteBatch?.Dispose();
            _texture?.Dispose();

            base.Dispose(disposing);
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
        await using DesktopRunner runner =
#if DEBUG
            new DebugRunner();
#else
            new();
#endif

        await runner.RunAsync(args).ConfigureAwait(false);
    }
}