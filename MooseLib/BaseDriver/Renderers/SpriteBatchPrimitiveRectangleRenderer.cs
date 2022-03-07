using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchPrimitiveRectangleRenderer : SpriteBatchRenderer
{
    private List<Color> Palette { get; } = new();

    public int TileWidth { get; private set; }
    public int TileHeight { get; private set; }

    public SpriteBatchPrimitiveRectangleRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, params Color[] colors) : base(spriteBatch)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Palette.AddRange(colors);
    }

    public override void Draw(MooseGame game, GameTime _gameTime, ILayer layer, int _layerNumber)
    {
        if (layer is not TileLayer<int> tileLayer)
            throw new Exception("TileLayer<int> layer expected");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
                SpriteBatch.FillRectangle(
                    i * TileWidth, j * TileHeight,
                    TileWidth, TileHeight,
                    Palette[tileLayer.Tiles[i, j]]);
    }
}
