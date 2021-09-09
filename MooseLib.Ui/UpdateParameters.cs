using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public record UpdateParameters(GameTime GameTime, Vector2 LocalMousePosition, GraphicsDevice GraphicsDevice)
    {
        public bool MouseOver { get; set; }

        public bool LeftMouse { get; set; }

        public bool RightMouse { get; set; }

        public Rectangle MouseRectangle => new((int)LocalMousePosition.X, (int)LocalMousePosition.Y, 1, 1);
    }
}
