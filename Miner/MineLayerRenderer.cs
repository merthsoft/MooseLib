using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.Miner
{
    class MineLayerRenderer : SpriteBatchTextureRenderer
    {
        public MineLayerRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites) : base(spriteBatch, tileWidth, tileHeight, sprites, 0, 0)
        {

        }

        public override void Draw(GameTime _, ILayer layer, int layerNumber)
        {
            if (layer is not MineLayer mineLayer)
                throw new Exception("TileLayer<int> layer expected");

            for (int i = 0; i < mineLayer.Width; i++)
                for (int j = 0; j < mineLayer.Height; j++)
                {
                    DrawSprite((int)mineLayer.GetDirtTile(i, j), i, j, layerNumber, mineLayer, 0);
                    DrawSprite((int)mineLayer.GetInteractiveTile(i, j), i, j, layerNumber, mineLayer, 1);
                }
        }
    }
}
