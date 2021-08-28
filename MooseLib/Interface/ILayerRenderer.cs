using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.MooseEngine.Interface
{
    public interface ILayerRenderer
    {
        void Update(GameTime gameTime);

        void Load(IMap map);

        void Begin(
            SpriteSortMode sortMode = SpriteSortMode.Deferred, 
            BlendState? blendState = null,
            SamplerState? samplerState = null, 
            DepthStencilState? depthStencilState = null, 
            RasterizerState? rasterizerState = null, 
            Effect? effect = null, 
            Matrix? transformMatrix = null);

        void Draw(GameTime gameTime, ILayer layer, int layerNumber);

        void End();
    }
}
