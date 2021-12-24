using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public abstract class Control
    {
        public object? Object { get; set; }

        public Window Window { get; }

        public Theme Theme => Window.Theme;

        public Color? Color { get; set; }

        public Vector2 Position { get; set; }

        public int FontIndex { get; set; }
        public SpriteFont Font => Theme.Fonts[FontIndex];
        
        public bool IsHidden { get; set; }

        protected UpdateParameters UpdateParameters { get; set; } = null!;


        public Rectangle Rectangle
        {
            get
            {
                var size = CalculateSize();
                return new((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);
            }
        }

        public Action<Control, UpdateParameters>? Action { get; set; }

        public Control(Window window, int x, int y)
        {
            Window = window;
            Position = new(x, y);
        }

        public Control Hide()
        {
            IsHidden = true;
            return this;
        }

        public Control Show()
        {
            IsHidden = false;
            return this;
        }

        public abstract Vector2 CalculateSize();

        public virtual void Update(UpdateParameters updateParameters)
            => UpdateParameters = updateParameters;

        public abstract void Draw(SpriteBatch spriteBatch, Vector2 parentOffset);

        public Vector2 CenterInWindow()
        {
            var size = CalculateSize();
            return Position = new(Window.Width / 2 - size.X / 2, Window.Height / 2 - size.Y / 2);
        }
    }
}
