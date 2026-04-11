using Microsoft.Xna.Framework;

namespace DefaultEngine.Editor.Internal.Plugins.MonoGamePlugin;

internal sealed class GameWrapper : Game
{
    public Game? Game { get; set; }

    //protected override bool BeginDraw() => _game.BeginDraw();

    //protected override void BeginRun() => _game.BeginRun();
}
