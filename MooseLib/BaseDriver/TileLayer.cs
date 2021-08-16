using System;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class TileLayer<TTile> : ITileLayer 
    {
        public int Height { get; }
        public int Width { get; }

        public string Name { get; }
        public int Number { get; }
        
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public string RendererKey { get; set; } = SpriteBatchPrimitiveRectangleRenderer.DefaultRenderKey;

        public TTile[,] Tiles { get; }

        public TileLayer(string name, int number, int width, int height)
        {
            Name = name;
            Number = number;
            Width = width;
            Height = height;

            Tiles = new TTile[Width, Height];
        }

        public ITile GetTile(int x, int y)
            => new SimpleTileContainer<TTile>(Tiles[x, y]);
    }
}
