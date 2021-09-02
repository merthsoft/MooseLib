using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.MooseEngine.BaseDriver.Renderers
{
    public class SpriteBatchObjectRenderer : ILayerRenderer
    {
        public SpriteBatch SpriteBatch { get; }

        public SpriteBatchObjectRenderer(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }

        public virtual void Draw(GameTime _, ILayer layer, int layerNumber, Matrix viewMatrix)
        {
            if (layer is not IObjectLayer objectLayer)
                throw new Exception("Object layer expected");

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, transformMatrix: viewMatrix);

            foreach (var obj in objectLayer)
            {
                var (texture, sourceRect) = obj.GetTexture();
                SpriteBatch.Draw(texture, obj.WorldPosition, sourceRect, Color.Transparent);
            }
                

            SpriteBatch.End();
        }

        public void Load(IMap map) { }

        public void Update(GameTime gameTime) { }
    }
}
