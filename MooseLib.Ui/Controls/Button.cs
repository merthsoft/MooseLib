using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Button : Control
{
    public string Text { get; set; }

    public Color? BackgroundColor { get; set; }
    public Color? BorderColor { get; set; }

    public int? WidthOverride { get; set; }
    public int? HeightOverride { get; set; }

    public Button(Theme theme, int x, int y, string text) : base(theme, x, y)
        => Text = text;

    public override Vector2 CalculateSize()
    {
        var fontSize = MeasureString(Text.Length == 0 ? "X" : Text);
        return new(WidthOverride ?? fontSize.X + 5, HeightOverride ?? fontSize.Y + 2);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        => DrawButton(spriteBatch, drawOffset,
            BackgroundColor ?? Theme.ResolveBackgroundColor(UpdateParameters, Enabled),
            BorderColor ?? Theme.ControlBorderColor,
            Theme.ResolveTextColor(UpdateParameters, Enabled, false)
           );

    protected void DrawButton(SpriteBatch spriteBatch, Vector2 drawOffset, Color backgroundColor, Color borderColor, Color textColor)
    {
        var (x, y) = Position + drawOffset;
        var (w, h) = CalculateSize();
        spriteBatch.FillRectangle(x, y, w, h, backgroundColor);
        spriteBatch.DrawRectangle(x, y, w, h, borderColor);
        spriteBatch.DrawString(Font, Text, new(x + 3, y + 1), textColor);
    }
}
