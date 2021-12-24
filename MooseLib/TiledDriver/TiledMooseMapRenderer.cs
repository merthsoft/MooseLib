using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled.Renderers;

namespace Merthsoft.Moose.MooseEngine.TiledDriver
{
    public record TiledMooseMapRenderer : ILayerRenderer
    {
        private TiledMapRenderer MapRenderer { get; }
        private Matrix transformMatrix;

        public TiledMooseMapRenderer(GraphicsDevice graphicsDevice)
            => MapRenderer = new(graphicsDevice);

        public void Update(GameTime gameTime)
            => MapRenderer.Update(gameTime);

        public void Load(IMap map)
            => MapRenderer.LoadMap((map as TiledMooseMap)?.Map);

        public void Begin(Matrix transformMatrix)
            => this.transformMatrix = transformMatrix;

        public void Draw(GameTime _, ILayer layer, int __)
            => MapRenderer.Draw((layer as TiledMooseTileLayer)?.Layer, transformMatrix);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            MapRenderer.Dispose();
        }
        public void LoadContent(MooseContentManager contentManager)
        {
            throw new NotImplementedException();
        }
    }
}
