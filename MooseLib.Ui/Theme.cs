using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MooseLib.Ui
{
    public class Theme
    {
        public string Name { get; private set; }

        internal Rectangle[] TextureRects = new Rectangle[9];

        private Texture2D windowTexture = null!;

        public List<SpriteFont> Fonts { get; } = new();

        public Texture2D WindowTexture
        {
            get => windowTexture;
            set
            {
                windowTexture = value;
                for (var index = 0; index < 9; index++)
                    TextureRects[index] = new Rectangle((index % 3) * TileWidth, index / 3 * TileHeight, TileWidth, TileHeight);
            }
        }

        public Vector2 ControlDrawOffset { get; set; }

        public int TileWidth { get; private set; }

        public int TileHeight { get; private set; }

        public Color TextColor { get; set; } = Color.Black;

        public Color TextMouseOverColor { get; set; } = Color.White;

        public Color SelectedColor { get; set; } = Color.Blue;

        public Theme(string name, Texture2D windowTexture, int tileWidth, int tileHeight, IEnumerable<SpriteFont> fonts)
        {
            (Name, TileWidth, TileHeight, WindowTexture)
             = (name, tileWidth, tileHeight, windowTexture);
            Fonts.AddRange(fonts);
        }

        internal void DrawWindowTexture(SpriteBatch spriteBatch, int index, Vector2 position, int x, int y)
        {
            var sourceRect = TextureRects[index];
            var destRect = new Rectangle((int)position.X + x * TileWidth, (int)position.Y + y * TileHeight, TileWidth, TileHeight);
            spriteBatch.Draw(WindowTexture, destRect, sourceRect, Color.White);
        }
    }
}