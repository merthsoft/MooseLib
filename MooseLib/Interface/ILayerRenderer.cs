using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.MooseEngine.Interface
{
    public interface ILayerRenderer : IDisposable
    {
        void Update(GameTime gameTime) { }

        void Begin(Matrix transformMatrix) { }
        void Draw(GameTime gameTime, ILayer layer, int layerNumber);
        void End() { }
    }
}
