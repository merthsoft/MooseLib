using Microsoft.Xna.Framework;

namespace Merthsoft.MooseEngine.Interface
{
    public interface ILayerRenderer
    {
        void Update(GameTime gameTime) { }

        void Load(IMap map) { }

        void Draw(GameTime gameTime, ILayer layer, int layerNumber, Matrix transformMatrix);
    }
}
