using Microsoft.Xna.Framework;

namespace MooseLib.Ui
{
    public class UpdateParameters
    {
        public GameTime GameTime { get; set; }
        public bool MouseOver { get; set; }
        public Vector2 LocalMousePosition { get; set; }
        public bool LeftMouse { get; set; }
        public bool RightMouse { get; set; }

        public UpdateParameters(GameTime gameTime, Vector2 localMousePosition)
            => (GameTime, LocalMousePosition)
             = (gameTime, localMousePosition);
    }
}
