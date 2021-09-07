using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers
{
    public class SpriteBatchObjectRenderer : ILayerRenderer
    {
        public SpriteBatch SpriteBatch { get; }
        public Effect? Effect { get; set; }

        public SpriteBatchObjectRenderer(SpriteBatch spriteBatch)
            => SpriteBatch = spriteBatch;

        public virtual void Begin(Matrix viewMatrix)
            => SpriteBatch.Begin(
                SpriteSortMode.FrontToBack, 
                BlendState.AlphaBlend, 
                SamplerState.PointClamp, 
                effect: Effect, 
                transformMatrix: viewMatrix);

        public virtual void Draw(GameTime _, ILayer layer, int layerNumber)
        {
            if (layer is not IObjectLayer objectLayer)
                throw new Exception("Object layer expected");

            foreach (var obj in objectLayer)
            {
                var drawParameters = obj.GetDrawParameters();
                SpriteBatch.Draw(drawParameters.Texture,
                    drawParameters.DestinationRectangle,
                    drawParameters.SourceRectangle,
                    drawParameters.Color ?? Color.White,
                    drawParameters.Rotation,
                    drawParameters.Origin ?? Vector2.Zero,
                    drawParameters.Effects,
                    drawParameters.LayerDepth);
            }
        }

        public virtual void End()
            => SpriteBatch.End();

        public void Update(GameTime gameTime) { }

        public void Dispose()
            => GC.SuppressFinalize(this);
    }
}
