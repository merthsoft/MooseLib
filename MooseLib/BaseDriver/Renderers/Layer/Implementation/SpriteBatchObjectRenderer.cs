using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;

public class SpriteBatchObjectRenderer : SpriteLayerBatchRenderer
{
    public SpriteBatchObjectRenderer(SpriteBatch spriteBatch)
        : base(spriteBatch) { }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer)
    {
        if (layer is not IObjectLayer objectLayer)
            throw new Exception("Object layer expected");

        foreach (var obj in objectLayer)
            obj.Draw(game, gameTime, SpriteBatch); // TODO: Draw offset
    }
}
