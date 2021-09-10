using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers
{
    public abstract class SpriteBatchRenderer : ILayerRenderer
    {
        public SpriteBatch SpriteBatch { get; }

        protected SpriteBatchRenderer(SpriteBatch spriteBatch)
            => SpriteBatch = spriteBatch;

        public Effect? Effect { get; set; }

        public virtual void Begin(Matrix viewMatrix)
            => SpriteBatch.Begin(
                SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                effect: Effect,
                transformMatrix: viewMatrix);

        public abstract void Draw(GameTime gameTime, ILayer layer, int layerNumber);
        public virtual void Update(GameTime gameTime) { }

        public virtual void End()
            => SpriteBatch.End();

        public void Dispose()
            => GC.SuppressFinalize(this);
    }
}
