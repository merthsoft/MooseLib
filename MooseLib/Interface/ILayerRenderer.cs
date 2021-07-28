using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MooseLib.Interface
{
    public interface ILayerRenderer
    {
        void Update(GameTime gameTime);

        void Load(IMap map);

        void Begin(
            SpriteSortMode? sortMode = null, 
            BlendState? blendState = null,
            SamplerState? samplerState = null, 
            DepthStencilState? depthStencilState = null, 
            RasterizerState? rasterizerState = null, 
            Effect? effect = null, 
            Matrix? transformMatrix = null);

        void Draw(ILayer layer);

        void End();
    }
}
