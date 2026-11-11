using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DefaultApplication.Controls.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DefaultEngine.Editor.Internal.Plugins.MonoGamePlugin;

[DataTemplate<Game>]
internal sealed class MonoGameControl : Control, IDisposable
{
    public static readonly DirectProperty<MonoGameControl, Game?> GameProperty =
        AvaloniaProperty.RegisterDirect<MonoGameControl, Game?>(
            nameof(Game),
            o => o.Game,
            (o, v) => o.Game = v);

    private readonly Stopwatch _stopwatch = new();
    private readonly GameTime _gameTime = new();
    private readonly PresentationParameters _presentationParameters = new()
    {
        BackBufferWidth = 1,
        BackBufferHeight = 1,
        BackBufferFormat = SurfaceFormat.Color,
        DepthStencilFormat = DepthFormat.Depth24,
        PresentationInterval = PresentInterval.Immediate,
        IsFullScreen = false
    };

    private readonly ILogger<MonoGameControl> _logger;
    private readonly Lock _lock;

    private CancellationTokenSource? _shutdownToken;

    private byte[] _bufferData = [];
    private WriteableBitmap? _bitmap;
    private bool _isInitialized;
    private Game? _game;
    private FormattedText _fps;

    private Stopwatch _frameRateStopwatch;
    private int _frameCount;

    protected override void OnDataContextChanged(EventArgs e)
    {
        Game = DataContext as Game;

        base.OnDataContextChanged(e);
    }

    public Game? Game
    {
        get => _game;
        set
        {
            if (_game == value)
                return;
            _game = value;

            if (_isInitialized)
            {
                Initialize();
            }
        }
    }

    public MonoGameControl(ILogger<MonoGameControl> logger)
    {
        _logger = logger;
        _lock = new Lock();

        Focusable = true;
        _fps = new FormattedText("0", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, Typeface.Default, 20, Brushes.Black);
        _frameRateStopwatch = Stopwatch.StartNew();
    }

    private bool HandleDeviceReset(GraphicsDevice device)
    {
        if (_presentationParameters.DeviceWindowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (device.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset)
        {
            ResetDevice(device, Bounds.Size);
        }

        return device.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal;
    }

    private void Initialize()
    {
        if (this.GetPresentationSource()?.RootVisual?.Parent is Window { PlatformImpl: { } } window && window.TryGetPlatformHandle()?.Handle is { } handle)
        {
            _presentationParameters.DeviceWindowHandle = handle;
        }

        if (Game is not { } game)
        {
            return;
        }

        if (game.GraphicsDevice is { } device)
        {
            ResetDevice(device, Bounds.Size);
        }

        RunFrame(game);
    }

    private void Start()
    {
        if (_isInitialized)
        {
            return;
        }

        Initialize();
        _stopwatch.Start();
        _isInitialized = true;
    }

    private void ResetDevice(GraphicsDevice device, Size newSize)
    {
        lock (_lock)
        {
            int newWidth = Math.Max(1, (int)Math.Ceiling(newSize.Width));
            int newHeight = Math.Max(1, (int)Math.Ceiling(newSize.Height));

            device.Viewport = new Viewport(0, 0, newWidth, newHeight);
            _presentationParameters.BackBufferWidth = newWidth;
            _presentationParameters.BackBufferHeight = newHeight;

            if (_presentationParameters.DeviceWindowHandle != IntPtr.Zero)
            {
                device.Reset(_presentationParameters);
            }


            _bitmap?.Dispose();
            _bitmap = new WriteableBitmap(
                new PixelSize(device.Viewport.Width, device.Viewport.Height),
                new Vector(96d, 96d),
                PixelFormat.Rgba8888,
                AlphaFormat.Opaque);
        }
    }

    private void RunFrame(Game game)
    {
        //_gameTime.ElapsedGameTime = _stopwatch.Elapsed;
        //_gameTime.TotalGameTime += _gameTime.ElapsedGameTime;
        //_stopwatch.Restart();

        try
        {
            game.RunOneFrame();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Dispatcher.Post(InvalidateVisual, DispatcherPriority.Render);
        }
    }

    private void CaptureFrame(GraphicsDevice device, WriteableBitmap bitmap)
    {
        using ILockedFramebuffer bitmapLock = bitmap.Lock();
        int size = bitmapLock.RowBytes * bitmapLock.Size.Height;

        if (_bufferData.Length < size)
        {
            Array.Resize(ref _bufferData, size);
        }

        device.GetBackBufferData(_bufferData, 0, size);
        Marshal.Copy(_bufferData, 0, bitmapLock.Address, size);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispose();

        base.OnDetachedFromVisualTree(e);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        finalSize = base.ArrangeOverride(finalSize);

        if (finalSize != _bitmap?.Size && Game?.GraphicsDevice is { } device)
        {
            ResetDevice(device, finalSize);
        }

        return finalSize;
    }

    public override async void Render(DrawingContext context)
    {
        lock (_lock)
        {
            if (Game is not { } game
            || Game.GraphicsDevice is not { } device
            || _bitmap is null
            || Bounds is { Width: < 1, Height: < 1 }
            || !HandleDeviceReset(device))
            {
                context.DrawRectangle(Brushes.Black, null, new Rect(Bounds.Size));

                return;
            }

            RunFrame(game);
            CaptureFrame(device, _bitmap);

            context.DrawImage(_bitmap, new Rect(_bitmap.Size), Bounds);
        }

        ++_frameCount;
        if (_frameRateStopwatch.Elapsed.TotalSeconds > 1)
        {
            _fps = new FormattedText((_frameCount / _frameRateStopwatch.Elapsed.TotalSeconds).ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, Typeface.Default, 20, Brushes.Black);
            _frameCount = 0;
            _frameRateStopwatch.Restart();
        }

        context.DrawText(_fps, default);

        Dispatcher.Post(InvalidateVisual, DispatcherPriority.Render);
    }

    public void Dispose()
    {
        _bitmap?.Dispose();
        _bitmap = null;
    }
}