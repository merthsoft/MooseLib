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

        public virtual void Draw(GameTime _, ILayer layer, int layerNumber)
        {
            if (layer is not IObjectLayer objectLayer)
                throw new Exception("Object layer expected");

            foreach (var obj in objectLayer)
                obj.Draw(SpriteBatch);
        }

        public void Load(IMap map) { }

        public void Update(GameTime gameTime) { }
    }
}
