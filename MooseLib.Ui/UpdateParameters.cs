using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public record UpdateParameters(GameTime GameTime, Vector2 LocalMousePosition)
    {
        public bool MouseOver { get; set; }

        public bool LeftMouse { get; set; }

        public bool RightMouse { get; set; }
    }
}
