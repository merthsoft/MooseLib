using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MooseLib.Ui.Controls
{
    public abstract class Control
    {
        public object? Object { get; set; }
        public Window Window { get; }
        public Theme Theme => Window.Theme;
        public Vector2 Position { get; set; }
        public int FontIndex { get; set; }

        protected UpdateParameters UpdateParameters { get; set; } = null!;

        public Rectangle Rectangle
        {
            get
            {
                var size = CalculateSize();
                return new((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);
            }
        }

        protected Vector2 GlobalPosition => GetGlobalPosition(Position);
        protected Vector2 GetGlobalPosition(Vector2 position) => position + Window.Position + Window.Theme.ControlDrawOffset;

        public Action<Control, UpdateParameters>? Action { get; set; }

        public Control(Window window)
            => Window = window;

        public Control(Window window, int x, int y) : this(window)
            => Position = new(x, y);

        public abstract Vector2 CalculateSize();

        public virtual void Update(UpdateParameters updateParameters)
            => UpdateParameters = updateParameters;

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
