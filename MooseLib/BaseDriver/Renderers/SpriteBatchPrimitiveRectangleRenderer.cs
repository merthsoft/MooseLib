using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.MooseEngine.BaseDriver.Renderers
{
    public class SpriteBatchPrimitiveRectangleRenderer : SpriteBatchObjectRenderer
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

        public override void Draw(GameTime _, ILayer layer, int layerNumber, Matrix viewMatrix)
        {
            if (layer is not TileLayer<int> tileLayer)
                throw new Exception("TileLayer<int> layer expected");

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, transformMatrix: viewMatrix);

            for (int i = 0; i < tileLayer.Width; i++)
                for (int j = 0; j < tileLayer.Height; j++)
                    SpriteBatch.FillRectangle(
                        i * TileWidth, j * TileHeight, 
                        TileWidth, TileHeight, 
                        Palette[tileLayer.Tiles[i, j]]);

            SpriteBatch.End();
        }
    }
}
