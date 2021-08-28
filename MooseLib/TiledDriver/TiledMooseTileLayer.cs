using Merthsoft.MooseEngine.Interface;
using MonoGame.Extended.Tiled;

namespace Merthsoft.MooseEngine.TiledDriver
{
    public record TiledMooseTileLayer(TiledMapTileLayer Layer) : ITileLayer<TiledMapTile>
    {
        public string Name => Layer.Name;
        public bool IsVisible
        {
            get => Layer.IsVisible;
            set => Layer.IsVisible = value;
        }

        public float Opacity
        {
            get => Layer.Opacity;
            set => Layer.Opacity = value;
        }

        public string RendererKey { get; set; } = TiledMooseMapRenderer.DefaultRenderKey;
        public int Number => throw new NotImplementedException();

        public ITile<TiledMapTile> GetTile(int x, int y)
            => new TiledMooseTile(Layer.GetTile((ushort)x, (ushort)y));

        public TiledMapTile GetTileValue(int x, int y)
            => Layer.GetTile((ushort)x, (ushort)y);

        public bool IsBlockedAt(int x, int y, IMap map)
            => GetTile(x, y).IsBlocking(map);
    }
}
