using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MooseLib.Interface;
using System;

namespace MooseLib.BaseDriver
{
    public class SpriteBatchRenderer : ILayerRenderer
    {
        public const string DefaultRenderKey = "SpriteBatchRenderer_Default";

        public SpriteBatch SpriteBatch { get; }

        public SpriteBatchRenderer(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }

        public void Begin(SpriteSortMode? sortMode = null, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformMatrix = null)
            => SpriteBatch.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        public void Draw(ILayer layer)
        {
            // TODO: Handle a tile layer if you can
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
