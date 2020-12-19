using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MooseLib.Ui
{
    public abstract class Control
    {
        public object? Object { get; set; }
        public Window Window { get; }
        public Vector2 Position { get; set; }

        protected UpdateParameters UpdateParameters { get; set; } = null!;

        public Rectangle Rectangle
        {
            get
            {
                var size = CalculateSize();
                return new((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);
            }
        }

        protected Vector2 GlobalPosition => Position + Window.Position + Window.WindowManager.ControlDrawOffset;

        public Action<Control, UpdateParameters>? Action { get; set; }

        public Control(Window window)
            => Window = window;


        public abstract Vector2 CalculateSize();

        public virtual void Update(UpdateParameters updateParameters)
            => UpdateParameters = updateParameters;

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
