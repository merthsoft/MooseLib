using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class Checkbox : Control
    {
        public string? Text { get; set; }
        public bool IsChecked { get; set; }

        public int? WidthOverride { get; set; }
        public int? HeightOverride { get; set; }

        public Checkbox(Window window, int x, int y) : base(window, x, y) { }

        public override Vector2 CalculateSize()
        {
            var width = WidthOverride ?? Theme.TileDrawWidth;
            var height = HeightOverride ?? Theme.TileDrawHeight;

            if (Text != null)
            {
                var textSize = Font.MeasureString(Text);
                if (height < textSize.Y)
                    height = (int)textSize.Y + 2;
                width = height + (int)textSize.X + 1;
            }

            return new(width, height);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            var labelLength = Text == null ? 0 : (int)Font.MeasureString(Text).X;
            var (x, y) = Position + drawOffset;
            var (w, h) = CalculateSize();

            if (WidthOverride == null)
                w -= labelLength;

            spriteBatch.FillRectangle(x, y, w, h, Theme.ResolveBackgroundColor(UpdateParameters, Enabled));
            spriteBatch.DrawRectangle(x, y, w, h, Theme.ControlBorderColor);

            if (IsChecked)
            {
                spriteBatch.DrawLine(x + 1, y + 1, x + w - 1, y + h - 1, Theme.SelectedColor, 2);
                spriteBatch.DrawLine(x + w - 1, y + 1, x + 1, y + h - 1, Theme.SelectedColor, 2);
            }
            
            if (Text != null)
                spriteBatch.DrawString(Font, Text, new(x + w + 1, y - 2), Theme.ResolveTextColor(UpdateParameters, Enabled, false));
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
