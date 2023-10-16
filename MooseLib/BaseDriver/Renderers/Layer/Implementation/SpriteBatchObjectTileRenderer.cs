using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;
public class SpriteBatchObjectTileRenderer : SpriteBatchAbstractTileRenderer
{
    public SpriteBatchObjectTileRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites, int textureMargin = 0, int tilePadding = 0)
        : base(spriteBatch, tileWidth, tileHeight, sprites, textureMargin, tilePadding)
    {
    }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer)
    {
        if (layer is not IObjectLayer objectLayer)
            throw new Exception("Object layer expected");

        foreach (var obj in objectLayer)
        {
            var drawIndex = obj.DrawIndex;
            if (drawIndex < 0)
                continue;
            var (x, y) = obj.Cell;
            DrawSprite(drawIndex, x, y, layer, DrawOffset);
        }
    }

}
