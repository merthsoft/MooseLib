using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MooseEngine.TiledDriver
{
    public record TiledMooseTileLayer(TiledMapTileLayer Layer) : ITileLayer<TiledMapTile>
    {
        public string Name => Layer.Name;
        public bool IsHidden
        {
            get => Layer.IsVisible;
            set => Layer.IsVisible = value;
        }

        public float Opacity
        {
            get => Layer.Opacity;
            set => Layer.Opacity = value;
        }
        public int Width => Layer.Width;
        public int Height => Layer.Height;

        public Vector2 DrawOffset { get; set; }

        public ITile<TiledMapTile> GetTile(int x, int y)
            => new TiledMooseTile(Layer.GetTile((ushort)x, (ushort)y));

        public TiledMapTile GetTileValue(int x, int y)
            => Layer.GetTile((ushort)x, (ushort)y);

        public bool IsBlockedAt(int x, int y, IMap map)
            => GetTile(x, y).IsBlocking(map);
    }
}
