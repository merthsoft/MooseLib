using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public class SpriteBatchIndexedTextureTileRenderer : SpriteBatchObjectRenderer
    {
        public new static string DefaultRenderKey = "SpriteBatchIndexedTileRenderer_Default";

        private Texture2D SpriteSheet { get; }

        public Color Color { get; set; } = Color.White;
        public float Rotation { get; set; }
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public int TileWidth { get; }
        public int TileHeight { get; }
        public int TileMargin { get; }
        public int TilePadding { get; }

        public SpriteBatchIndexedTextureTileRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites, int tileMargin = 0, int tilePadding = 0) : base(spriteBatch)
        {
            SpriteSheet = sprites;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            TileMargin = tileMargin;
            TilePadding = tilePadding;
        }


        public override void Draw(GameTime _, ILayer layer, int layerNumber)
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
                destinationRectangle: new(i * TileWidth, j * TileHeight, TileWidth, TileHeight),
                sourceRectangle: new(sourceX, sourceY, TileWidth, TileHeight),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: 0);
        }
    }
}
