using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchObjectRenderer : SpriteBatchRenderer
{
    public SpriteBatchObjectRenderer(SpriteBatch spriteBatch)
        : base(spriteBatch) { }

    public override void Draw(GameTime _, ILayer layer, int layerNumber)
    {
        if (layer is not IObjectLayer objectLayer)
            throw new Exception("Object layer expected");

        foreach (var obj in objectLayer)
            obj.Draw(SpriteBatch);
    }
}
