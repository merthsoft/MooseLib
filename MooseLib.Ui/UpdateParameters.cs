using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public record UpdateParameters(GameTime GameTime, Vector2 LocalMousePosition, MouseState? RawMouseState, KeyboardState? RawKeyState)
    {
        public bool MouseOver { get; set; }

        public bool LeftMouseClick { get; set; }

        public bool RightMouseClick { get; set; }

        public bool LeftMouseDown { get; set; }

        public bool RightMouseDown { get; set; }
    }
}
