using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MooseLib.Ui
{
    public class Line : Control
    {
        public Vector2 End { get; set; }
        public int Thickness { get; set; } = 1;

        public override Vector2 CalculateSize() => new(Position.X - End.X, Position.Y - End.Y);

        public Line(Window window, int x1, int y1, int x2, int y2, int thickness = 1) : base(window, x1, y1)
        {
            End = new(x2, y2);
            Thickness = thickness;
        }

        public override void Draw(SpriteBatch spriteBatch)
            => spriteBatch.DrawLine(GlobalPosition, GetGlobalPosition(End), Theme.TextColor, Thickness);
    }
}
