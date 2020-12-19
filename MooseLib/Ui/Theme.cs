using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MooseLib.Ui
{
    public class Theme
    {
        public string Name { get; private set; }

        internal Rectangle[] TextureRects = new Rectangle[9];

        private Texture2D windowTexture = null!;

        public SpriteFont Font { get; set; }
        public Texture2D WindowTexture
        {
            get => windowTexture;
            set
            {
                windowTexture = value;

                TileWidth = WindowTexture.Width / 3;
                TileHeight = WindowTexture.Height / 3;
                for (var index = 0; index < 9; index++)
                    TextureRects[index] = new Rectangle(index % 3 * TileWidth, index / 3 * TileHeight, TileWidth, TileHeight);
            }
        }
        public Vector2 ControlDrawOffset { get; set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public Color TextColor { get; set; } = Color.Black;
        public Color TextMouseOverColor { get; set; } = Color.White;
        public Color SelectedColor { get; set; } = Color.Blue;

        public Theme(string name, Texture2D windowTexture, SpriteFont font)
        => (Name, WindowTexture, Font)
         = (name, windowTexture, font);
    }
}