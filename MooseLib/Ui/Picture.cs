using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MooseLib.Ui
{
    public class Picture : Control
    {
        public Texture2D Texture { get; }
        private Rectangle SourceRectangle { get; }
        public Vector2 Scale { get; set; } = Vector2.One;

        public float Rotation { get; set; }
        public SpriteEffects SpriteEffects { get; set; }

        public override Vector2 CalculateSize() => new(Texture.Width, Texture.Height);
        
        public Picture(Window window, Texture2D texture) : base(window)
        {
            Texture = texture;
            SourceRectangle = new(0, 0, Texture.Width, Texture.Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
            => spriteBatch.Draw(Texture, GlobalPosition, SourceRectangle, Color.White, Rotation, Vector2.Zero, Scale, SpriteEffects, 1);

    }
}
