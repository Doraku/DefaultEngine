using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DefaultEngine.Editor.Internal.Plugins.MonoGamePlugin;

/// <summary>
/// The methods for resizing the screen
/// </summary>
public enum ResizeMethod
{
    /// <summary>
    /// Just stretch/shrink the image in x/y to fit
    /// </summary>
    Stretch,
    /// <summary>
    /// Stretch/shrink the image by the smaller scale-difference in x/.
    /// Keeps aspect-ratio.
    /// This results in black bars around the image.
    /// </summary>
    Pillow,
    /// <summary>
    /// Stretch/shrink the image by the bigger scale-difference.
    /// Keeps aspect-ratio.
    /// Image will most likeley be bigger than the window
    /// </summary>
    Fill
}

/// <summary>
/// Used to render independent from the screen's size
/// </summary>
public sealed class ResolutionRenderer : IDisposable
{
    /// <summary>
    /// The resolution that will be rendered on
    /// </summary>
    public Point VirtualResolution { get; set { field = value; _needsUpdate = true; } }

    /// <summary>
    /// The actual screen-size
    /// </summary>
    public Point ScreenResolution { get; set { field = value; _needsUpdate = true; } }

    /// <summary>
    /// Method to be used when resizing the image
    /// Default: Stretch
    /// </summary>
    public ResizeMethod Method { get; set { field = value; _needsUpdate = true; } }

    private Vector2 _scale;
    private Vector2 _methodScale;

    private readonly GraphicsDevice _device;
    private readonly SpriteBatch _batch;
    private RenderTarget2D? _target;

    private RenderTargetBinding[]? _prevTargets;

    private bool _needsUpdate;

    public Color DefaultColor { get; set; } = Color.CornflowerBlue;

    public ResolutionRenderer(Point virtualResolution, GraphicsDevice device)
    {
        _device = device;

        VirtualResolution = virtualResolution;
        ScreenResolution = _device.Viewport.Bounds.Size;

        Method = ResizeMethod.Stretch;

        _batch = new SpriteBatch(_device);
    }

    private void Update()
    {
        if (!_needsUpdate && ScreenResolution == _device.Viewport.Bounds.Size)
        {
            return;
        }

        _needsUpdate = false;

        ScreenResolution = _device.Viewport.Bounds.Size;
        _scale = ScreenResolution.ToVector2() / VirtualResolution.ToVector2();

        _target?.Dispose();

        _target = new RenderTarget2D(_device, VirtualResolution.X, VirtualResolution.Y);

        switch (Method)
        {
            case ResizeMethod.Stretch:
                _methodScale = _scale;
                break;
            case ResizeMethod.Pillow:
                float smaller = Math.Min(_scale.X, _scale.Y);
                _methodScale = new Vector2(smaller, smaller);
                break;
            case ResizeMethod.Fill:
                float bigger = Math.Max(_scale.X, _scale.Y);
                _methodScale = new Vector2(bigger, bigger);
                break;
            default:
                _methodScale = _scale;
                break;
        }
    }

    /// <summary>
    /// Call before drawing anything
    /// </summary>
    public void Begin()
    {
        Update();

        _prevTargets = _device.GetRenderTargets();
        _device.SetRenderTarget(_target);
        _device.Clear(DefaultColor);
    }

    /// <summary>
    /// Call after drawing is finished
    /// </summary>
    public void End()
    {
        Vector2 pos = ScreenResolution.ToVector2() / 2f;
        Vector2 origin = VirtualResolution.ToVector2() / 2f;

        _device.SetRenderTargets(_prevTargets);
        _device.Clear(DefaultColor);

        _batch.Begin();
        _batch.Draw(_target, pos, null, Color.White, 0, origin, _methodScale, SpriteEffects.None, 0);
        _batch.End();
    }

    public void Dispose()
    {
        _batch.Dispose();
        _target?.Dispose();
    }
}