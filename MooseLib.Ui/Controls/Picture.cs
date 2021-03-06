﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MooseLib.Ui.Controls
{
    public class Picture : Control
    {
        public Texture2D Texture { get; set; }

        private Rectangle? sourceRectangle;

        public Rectangle SourceRectangle
        {
            get => sourceRectangle ?? new(0, 0, Texture.Width, Texture.Height);
            set => sourceRectangle = value;
        }

        public Vector2 Scale { get; set; } = Vector2.One;

        public float Rotation { get; set; }

        public SpriteEffects SpriteEffects { get; set; }

        public override Vector2 CalculateSize() => new(Texture.Width, Texture.Height);

        public Picture(Window window, int x, int y, Texture2D texture) : base(window, x, y) => Texture = texture;

        public override void Draw(SpriteBatch spriteBatch)
            => spriteBatch.Draw(Texture, GlobalPosition, SourceRectangle, Color.White, Rotation, Vector2.Zero, Scale, SpriteEffects, 1);
    }
}
