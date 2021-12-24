using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.Miner
{
    public class MineLayerRenderer : SpriteBatchTextureRenderer, IDisposable
    {
        private static Color[] dirtColors = new[]
        {
            Color.Black,
            Color.DarkGray,
            Color.Gray,
            Color.LightGray,
            Color.SlateGray,
            Color.White,
            Color.Yellow,
            Color.DarkRed,
            Color.Red,
            Color.IndianRed,
            Color.Blue,
            Color.DeepSkyBlue,
            Color.SeaGreen,
            Color.DarkSeaGreen,
        }.Reverse().ToArray();

        // Set in LoadContent
        private RenderTarget2D dirtTexture = null!;

        public MineLayerRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites) : base(spriteBatch, tileWidth, tileHeight, sprites, 0, 0)
        {
        }

        public override void LoadContent(MooseContentManager contentManager)
        {
            var graphics = contentManager.GraphicsDevice;

            var game = contentManager.GetGame<MinerGame>()!;
            GenerateDirtTexture(graphics, game.MapWidth, game.MapHeight, game.TileWidth, game.TileHeight);
        }

        public void GenerateDirtTexture(GraphicsDevice graphics, int mapWidth, int mapHeight, int tileWidth, int tileHeight)
        {
            var width = mapWidth * tileWidth;
            var height = mapHeight * tileHeight;
            lock (graphics)
            {
                dirtTexture = new RenderTarget2D(graphics, width, height);
                graphics.SetRenderTarget(dirtTexture);
                graphics.Clear(Color.Transparent);
                var dirtTextureSpriteBatch = new SpriteBatch(graphics);
                dirtTextureSpriteBatch.Begin();

                for (int i = 0; i < mapWidth; i++)
                    for (int j = 0; j < mapHeight; j++)
                    {
                        var destRect = GetDestinationRectangle(i, j, Vector2.Zero);
                        
                        for (var colorIndex = 0; colorIndex < dirtColors.Length; colorIndex++)
                        {
                            var numSpots = Random.Shared.Next(0, 1 + (int)(colorIndex*1.25));
                            for (var spot = 0; spot < numSpots; spot++)
                            {
                                var x = Random.Shared.Next(destRect.X, destRect.X + destRect.Width);
                                var y = Random.Shared.Next(destRect.Y, destRect.Y + destRect.Height);

                                dirtTextureSpriteBatch.DrawPoint(x, y, dirtColors[colorIndex]);
                            }
                        }
                    }

                dirtTextureSpriteBatch.End();
                graphics.SetRenderTarget(null);
            }
        }

        public override void Draw(GameTime _, ILayer layer, int layerNumber)
        {
            if (layer is not MineLayer mineLayer)
                throw new Exception("MineLayer expected");

            for (int i = 0; i < mineLayer.Width; i++)
                for (int j = 0; j < mineLayer.Height; j++)
                {
                    if (mineLayer.GetDirtTile(i, j) == Tile.Dirt)
                    {
                        var destRect = GetDestinationRectangle(i, j, mineLayer.DrawOffset);
                        SpriteBatch.Draw(dirtTexture, destRect, destRect, Color.White);
                    }

                    DrawSprite((int)mineLayer.TileAt(i, j), i, j, layerNumber, mineLayer, 1);
                }
        }
    }
}
