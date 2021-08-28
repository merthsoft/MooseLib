using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class SpriteBatchObjectRenderer : ILayerRenderer
    {
        public static string DefaultRenderKey = "SpriteBatchObjectRenderer_Default";

        public SpriteBatch SpriteBatch { get; }

        public SpriteBatchObjectRenderer(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }

        public void Begin(
            SpriteSortMode sortMode = SpriteSortMode.Deferred,
            BlendState? blendState = null, SamplerState? samplerState = null,
            DepthStencilState? depthStencilState = null,
            RasterizerState? rasterizerState = null,
            Effect? effect = null,
            Matrix? transformMatrix = null)
            => SpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);

        public virtual void Draw(GameTime _, ILayer layer, int layerNumber)
        {
            if (layer is not IObjectLayer objectLayer)
                throw new Exception("Object layer expected");

            foreach (var obj in objectLayer)
                obj.Draw(SpriteBatch);
        }

        public void End()
            => SpriteBatch.End();

        public void Load(IMap map) { }

        public void Update(GameTime gameTime) { }
    }
}
