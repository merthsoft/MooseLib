using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled.Renderers;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.TiledDriver
{
    public record TiledMooseMapRenderer : ILayerRenderer
    {
        private TiledMapRenderer MapRenderer { get; }

        public TiledMooseMapRenderer(GraphicsDevice graphicsDevice)
            => MapRenderer = new(graphicsDevice);

        public void Load(IMap map)
            => MapRenderer.LoadMap((map as TiledMooseMap)?.Map);

        public void Draw(GameTime _, ILayer layer, int __, Matrix transformMatrix)
            => MapRenderer.Draw((layer as TiledMooseTileLayer)?.Layer, transformMatrix);
    }
}
