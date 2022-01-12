using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class Button : Control
    {
        public string Text { get; set; }
        public Button(Window window, int x, int y, string text) : base(window, x, y)
            => Text = text;

        public override Vector2 CalculateSize()
            => Font.MeasureString(Text) + new Vector2(4, 2);

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            var (x, y) = Position + drawOffset;
            var (w, h) = CalculateSize();
            spriteBatch.FillRectangle(x, y, w, h, Theme.ControlBackgroundColor);
            spriteBatch.DrawRectangle(x, y, w, h, Theme.ControlBorderColor);
            spriteBatch.DrawString(Font, Text, new(x + 2, y + 1), UpdateParameters.MouseOver ? Theme.TextMouseOverColor : Theme.TextColor);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouseClick)
                Action?.Invoke(this, updateParameters);
        }
    }
}
