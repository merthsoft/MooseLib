using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MooseLib.Ui
{
    public class Label : Control
    {
        public string? Text { get; set; }

        public Label(Window window) : base(window)
        {

        }

        public override Vector2 CalculateSize()
            => Window.Theme.Font.MeasureString(Text);

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Text == null || Window.Theme.Font == null)
                return;

            spriteBatch.DrawString(Window.Theme.Font, Text, GlobalPosition, UpdateParameters.MouseOver ? Theme.TextMouseOverColor : Theme.TextColor);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouse)
                Action?.Invoke(this, updateParameters);
            base.Update(updateParameters);
        }
    }
}
