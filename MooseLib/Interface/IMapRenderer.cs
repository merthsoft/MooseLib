using Microsoft.Xna.Framework;

namespace MooseLib.Interface
{
    public interface IMapRenderer
    {
        void LoadMap(IMap map);
        void Update(GameTime gameTime);
        public void Draw(ILayer layer, Matrix? viewMatrix = null);
        public void Draw(int layerIndex, Matrix? viewMatrix = null);
    }
}
