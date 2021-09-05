using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver
{
    public class TileLayer<TTile> : ITileLayer<TTile>
    {
        public int Height { get; }
        public int Width { get; }

        public string Name { get; }
        
        public bool IsVisible { get; set; } = true;
        public float Opacity { get; set; }

        public TTile[,] Tiles { get; }

        public TileLayer(string name, int width, int height)
        {
            Name = name;
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
