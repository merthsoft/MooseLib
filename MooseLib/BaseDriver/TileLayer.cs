using Merthsoft.MooseEngine.BaseDriver.Renderers;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class TileLayer<TTile> : ITileLayer<TTile>
    {
        public int Height { get; }
        public int Width { get; }

        public string Name { get; }
        public int Number { get; }
        
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public string RendererKey { get; set; } = DefaultRenderKeys.SpriteBatchPrimitiveRectangleRenderer;

        public TTile[,] Tiles { get; }

        public TileLayer(string name, int number, int width, int height)
        {
            Name = name;
            Number = number;
            Width = width;
            Height = height;

            Tiles = new TTile[Width, Height];
        }

        public ITile<TTile> GetTile(int x, int y)
            => new SimpleTileReference<TTile>(Tiles[x, y]);

        public TTile GetTileValue(int x, int y)
            => Tiles[x, y];

        public ITile<TTile> SetTile(int x, int y, TTile value)
        {
            Tiles[x, y] = value;
            return GetTile(x, y);
        }
    }
}
