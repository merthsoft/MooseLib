using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class SpriteBatchIndexedSpriteTileRenderer : SpriteBatchObjectRenderer
    {
        public new static string DefaultRenderKey = "SpriteBatchIndexedTileRenderer_Default";

        private Sprite[] SpriteSheet { get; }

        public Transform2 Transform { get; set; } = new();
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public SpriteBatchIndexedSpriteTileRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, IEnumerable<Sprite> sprites) : base(spriteBatch)
        {
            SpriteSheet = sprites.ToArray();
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }


        public override void Draw(ILayer layer, int layerNumber)
        {
            if (layer is not TileLayer<int> tileLayer)
                throw new Exception("TileLayer<int> layer expected");

            for (int i = 0; i < tileLayer.Width; i++)
                for (int j = 0; j < tileLayer.Height; j++)
                    DrawSprite(SpriteSheet[tileLayer.Tiles[i, j]], i, j, layerNumber);
        }

        public virtual void DrawSprite(Sprite sprite, int i, int j, int layer)
            => sprite.Draw(SpriteBatch, new(i * TileWidth, i * TileHeight), Transform, SpriteEffects);
    }
}
