using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled.Renderers;
using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.TiledDriver
{
    public record TiledMooseMapRenderer : ILayerRenderer
    {
        public const string DefaultRenderKey = "TiledMooseMapRenderer_Default";

        private TiledMapRenderer MapRenderer { get; }
        private Matrix? ViewMatrix { get; set; }

        public TiledMooseMapRenderer(GraphicsDevice graphicsDevice)
            => MapRenderer = new(graphicsDevice);

        public void Load(IMap map)
            => MapRenderer.LoadMap((map as TiledMooseMap)?.Map);

        public void Update(GameTime gameTime)
            => MapRenderer.Update(gameTime);

        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformMatrix = null)
            => ViewMatrix = transformMatrix;

        public void Draw(ILayer layer, int _)
            => MapRenderer.Draw((layer as TiledMooseTileLayer)?.Layer, ViewMatrix);

        public void End() { }
    }
}
