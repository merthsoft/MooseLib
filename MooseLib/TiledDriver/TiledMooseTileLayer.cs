using MonoGame.Extended.Tiled;
using MooseLib.Interface;

namespace MooseLib.Tiled
{
    public record TiledMooseTileLayer(TiledMapTileLayer Layer) : ITileLayer
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

        public ITile GetTile(int x, int y)
            => new TiledMooseTile(Layer.GetTile((ushort)x, (ushort)y));
    }
}
