using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;
public class SpriteBatchPrimitiveRectangleRenderer : SpriteLayerBatchRenderer
{
    private List<Color> Palette { get; } = [];

    public int TileWidth { get; private set; }
    public int TileHeight { get; private set; }

    public SpriteBatchPrimitiveRectangleRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, params Color[] colors) : base(spriteBatch)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Palette.AddRange(colors);
    }

    public override void Draw(MooseGame game, GameTime _gameTime, ILayer layer)
    {
        if (layer is not TileLayer<int> tileLayer)
            throw new Exception("TileLayer<int> layer expected");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
            {
                var color = Palette[tileLayer.Tiles[i, j]];
                if (color == Color.Transparent)
                    continue;

                SpriteBatch.FillRectangle(
                    i * TileWidth + DrawOffset.X, j * TileHeight + DrawOffset.Y,
                    TileWidth, TileHeight, color);
            }
    }
}
