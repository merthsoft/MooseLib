using System;
using MonoGame.Extended.Tiled;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.TiledDriver
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
        public int Number => throw new NotImplementedException();

        public ITile GetTile(int x, int y)
            => new TiledMooseTile(Layer.GetTile((ushort)x, (ushort)y));
    }
}
