using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class Slider : Control
    {
        public int Min { get; }
        public int Max { get; }
        public int Value { get; set; }

        public bool DrawLabel { get; set; } = true;

        public int? WidthOverride { get; set; } = null;
        public int? HeightOverride { get; set; } = null;

        public Slider(Window window, int x, int y, int min, int max) : base(window, x, y)
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
                var label = Value.ToString();
                var textSize = Font.MeasureString(label);
                width += (int)textSize.X + 1;
            }

            return new(width, height);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        {
            var width = WidthOverride ?? (Max - Min + 2);
            var height = CalculatedRectangle.Height;

            var (x, y) = Position + parentOffset;

            spriteBatch.FillRectangle(x, y, width, height, Theme.ControlBackgroundColor);
            spriteBatch.DrawRectangle(x, y, width, height, Theme.ControlBorderColor);

            var percentOfPosition = (float)Value / (Max - Min);
            var position = (int)((width - 2) * percentOfPosition);
            spriteBatch.DrawLine(x + position + 1, y + 1, x + position + 1, y + height - 1, Theme.ControlPointerColor);

            if (DrawLabel)
                spriteBatch.DrawString(Font, Value.ToString(), new(x + width + 1, y - 2), Theme.TextColor);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouseDown)
            {
                var newValue = (int)(Min + updateParameters.LocalMousePosition.X - 1);
                if (newValue >= Min && newValue <= Max)
                {
                    Value = newValue;
                    Action?.Invoke(this, updateParameters);
                }
            }
        }
    }
}
