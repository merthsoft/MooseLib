using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MooseLib.Ui
{
    public class Label : Control
    {
        public string? Text { get; set; }
        public Color Color { get; set; } = Color.Black;
        public Color MouseOverColor { get; set; } = Color.White;

        public Label(Window window) : base(window)
        {

        }

        public override Vector2 CalculateSize()
            => Window.WindowManager.Font.MeasureString(Text);

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Text == null || Window.WindowManager.Font == null)
                return;

            spriteBatch.DrawString(Window.WindowManager.Font, Text, GlobalPosition, UpdateParameters.MouseOver ? MouseOverColor : Color);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouse)
                Action?.Invoke(this, updateParameters);
            base.Update(updateParameters);
        }
    }
}
