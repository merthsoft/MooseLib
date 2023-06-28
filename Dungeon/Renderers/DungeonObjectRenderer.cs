using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

namespace Merthsoft.Moose.Dungeon.Renderers;

public class DungeonObjectRenderer : SpriteBatchRenderer
{
    public DungeonObjectRenderer(SpriteBatch spriteBatch)
        : base(spriteBatch) { }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer, Vector2 drawOffset)
    {
        var dungeonGame = (game as WiggleWizzardGame)!;
        if (layer is not IObjectLayer objectLayer)
            throw new Exception("Object layer expected");

        foreach (var obj in objectLayer)
            if (dungeonGame.Player.CanSee(obj.Position) == FogOfWar.None)
                obj.Draw(game, gameTime, SpriteBatch);
    }
}
