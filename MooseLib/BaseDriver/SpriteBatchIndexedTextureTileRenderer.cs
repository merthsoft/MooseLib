using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class SpriteBatchIndexedTextureTileRenderer : SpriteBatchObjectRenderer
    {
        public new static string DefaultRenderKey = "SpriteBatchIndexedTileRenderer_Default";

        private Texture2D SpriteSheet { get; }

        public Color Color { get; set; }
        public float Rotation { get; set; }
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public SpriteBatchIndexedTextureTileRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites) : base(spriteBatch)
        {
            SpriteSheet = sprites;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }


        public override void Draw(ILayer layer, int layerNumber)
        {
            if (layer is not TileLayer<int> tileLayer)
                throw new Exception("TileLayer<int> layer expected");

            for (int i = 0; i < tileLayer.Width; i++)
                for (int j = 0; j < tileLayer.Height; j++)
                    DrawSprite(tileLayer.Tiles[i, j], i, j, layerNumber);
        }

        public virtual void DrawSprite(int spriteIndex, int i, int j, int layer)
        {
            var columns = SpriteSheet.Width / TileWidth;

            var sourceX = (spriteIndex % columns) * TileWidth;
            var sourceY = (spriteIndex / columns) * TileHeight;

            SpriteBatch.Draw(SpriteSheet, 
                destinationRectangle: new(i * TileWidth, i * TileHeight, TileWidth, TileHeight),
                sourceRectangle: new(sourceX, sourceY, TileWidth, TileHeight),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layer);
        }
    }
}
