using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.MooseEngine.Ui.Controls
{
    public class Label : Control
    {
        public string? Text { get; set; }

        public bool HighlightOnHover { get; set; }

        public Label(Window window, int x, int y) : base(window, x, y)
        {
        }

        public override Vector2 CalculateSize()
            => Window.Theme.Fonts[FontIndex].MeasureString(Text);

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Text == null)
                return;

            spriteBatch.DrawString(Window.Theme.Fonts[FontIndex], Text, GlobalPosition, Color ?? 
                (HighlightOnHover && UpdateParameters.MouseOver ? Theme.TextMouseOverColor : Theme.TextColor));
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouse)
                Action?.Invoke(this, updateParameters);
            base.Update(updateParameters);
        }
    }
}
