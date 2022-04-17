using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Slider : Control
{
    public int Min { get; }
    public int Max { get; }
    public int Value { get; set; }

    public bool DrawLabel { get; set; } = true;
    public bool LeftLabel { get; set; } = false;

    public float? WidthOverride { get; set; } = null;
    public float? HeightOverride { get; set; } = null;

    public Slider(IControlContainer container, float x, float y, int min, int max) : base(container, x, y)
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

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset, GameTime gameTime)
    {
        var width = WidthOverride ?? (Max - Min + 2);
        var height = Rectangle.Height - 2;

        var (x, y) = Position + parentOffset;

        if (DrawLabel && LeftLabel)
        {
            var maxLength = Font.MeasureString(Max.ToString()).X + 3;
            var currentLength = Font.MeasureString(Value.ToString()).X + 3;
            spriteBatch.DrawString(Font, Value.ToString(), new(x + maxLength - currentLength, y), ResolvedTextColor);
            x += (int)Font.MeasureString(Max.ToString()).X + 3;
        }

        spriteBatch.FillRectangle(x, y, width, height, ResolvedBackgroundColor);
        spriteBatch.DrawRectangle(x, y, width, height, ResolvedBorderColor);

        var percentOfPosition = (float)Value / (Max - Min);
        var position = (int)((width - 5) * percentOfPosition);
        spriteBatch.FillRectangle(x + position + 1, y + 1, 3, height - 2, ResolvedPointerColor);

        if (DrawLabel && !LeftLabel)
            spriteBatch.DrawString(Font, Value.ToString(), new(x + width + 3, y), ResolvedTextColor);
    }

    public override void Update(UpdateParameters updateParameters)
    {
        var width = WidthOverride ?? (Max - Min + 2);
        var height = Rectangle.Height;

        var startX = 1;
        if (DrawLabel && LeftLabel)
            startX += (int)Font.MeasureString(Max.ToString()).X + 3;
        var sliderRect = new RectangleF(startX, 1, width - 2, height - 2);

        if (updateParameters.MouseOver && updateParameters.LeftMouseDown)
        {
            var mouseX = (int)updateParameters.LocalMousePosition.X;
            var difference = mouseX - startX;

            if (difference >= Min && difference <= Max)
            {
                Value = difference;
                Action?.Invoke(this, updateParameters);
            }
        }
    }
}
