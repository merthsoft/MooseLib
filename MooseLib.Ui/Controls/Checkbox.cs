using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class Checkbox : Control
    {
        public string? Text { get; set; }
        public bool IsChecked { get; set; }

        public Checkbox(Window window, int x, int y) : base(window, x, y) { }

        public override Vector2 CalculateSize()
        {
            var labelLength = Text == null ? 0 : Font.MeasureString(Text).X;
            return new(Theme.TileDrawWidth + labelLength, Theme.TileDrawHeight);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            var (x, y) = Position + drawOffset;
            var (w, h) = CalculateSize();

            var color = UpdateParameters.MouseOver ? Theme.TextMouseOverColor : Theme.TextColor;

            spriteBatch.FillRectangle(x, y, Theme.TileDrawWidth, Theme.TileDrawHeight, Theme.ControlBackgroundColor);
            spriteBatch.DrawRectangle(x, y, Theme.TileDrawWidth, Theme.TileDrawHeight, Theme.ControlBorderColor);

            if (IsChecked)
            {
                spriteBatch.DrawLine(x + 1, y + 1, x + Theme.TileDrawWidth - 1, y + Theme.TileDrawHeight - 1, Theme.SelectedColor, 2);
                spriteBatch.DrawLine(x + Theme.TileDrawWidth - 1, y + 1, x + 1, y + Theme.TileDrawHeight - 1, Theme.SelectedColor, 2);
            }
            
            if (Text != null)
                spriteBatch.DrawString(Font, Text, new(x + Theme.TileDrawWidth + 1, y - 2), color);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouseClick)
            {
                IsChecked = !IsChecked;
                Action?.Invoke(this, updateParameters);
            }
        }
    }
}
