using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class Slider : Control
    {
        public long Min { get; }
        public long Max { get; }
        public long Value { get; set; }

        public bool DrawLabel { get; set; } = true;
        public bool LeftLabel { get; set; } = true;

        public int? WidthOverride { get; set; } = null;
        public int? HeightOverride { get; set; } = null;

        public Color? TextColor { get; set; }
        public Color ResolvedTextColor => TextColor ?? Theme.ResolveTextColor(UpdateParameters, Enabled, false);

        public Slider(Window window, int x, int y, long min, long max) : base(window, x, y)
        {
            Min = min;
            Max = max;
        }

        public override Vector2 CalculateSize()
        {
            var width = WidthOverride ?? (Max - Min + 2);
            var height = HeightOverride ?? Theme.TileDrawHeight;

            if (DrawLabel)
            {
                var label = Max.ToString();
                var textSize = Font.MeasureString(label);
                width += (int)textSize.X + 3;
                if (height < textSize.Y)
                    height = (int)textSize.Y + 2;
            }

            return new(width, height);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        {
            var width = WidthOverride ?? (Max - Min + 2);
            var height = CalculatedRectangle.Height;

            var (x, y) = Position + parentOffset;
            
            if (DrawLabel && LeftLabel)
            {
                var maxLength = Font.MeasureString(Max.ToString()).X + 3;
                var currentLength = Font.MeasureString(Value.ToString()).X + 3;
                spriteBatch.DrawString(Font, Value.ToString(), new(x + maxLength - currentLength, y), ResolvedTextColor);
                x += Font.MeasureString(Max.ToString()).X + 3;
            }

            spriteBatch.FillRectangle(x, y, width, height, Theme.ResolveBackgroundColor(UpdateParameters, Enabled));
            spriteBatch.DrawRectangle(x, y, width, height, Theme.ControlBorderColor);

            var percentOfPosition = (float)Value / (Max - Min);
            var position = (int)((width - 5) * percentOfPosition);
            spriteBatch.FillRectangle(x + position + 1, y + 1, 3, height - 2, Theme.ControlPointerColor);

            if (DrawLabel && !LeftLabel)
                spriteBatch.DrawString(Font, Value.ToString(), new(x + width + 3, y), ResolvedTextColor);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            var width = WidthOverride ?? (Max - Min + 2);
            var height = CalculatedRectangle.Height;

            var startX = 1; 
            if (DrawLabel && LeftLabel)
                startX += (int)Font.MeasureString(Max.ToString()).X + 3;
            var sliderRect = new Rectangle(startX, 1, (int)width - 2, height - 2);

            if (updateParameters.MouseOver && updateParameters.LeftMouseDown)
            {
                var mouseX = updateParameters.LocalMousePosition.X;
                var difference = (long)(mouseX - startX);

                if (difference >= Min && difference <= Max)
                {
                    Value = difference;
                    Action?.Invoke(this, updateParameters);
                }
            }
        }
    }
}
