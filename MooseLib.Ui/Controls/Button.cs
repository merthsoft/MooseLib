namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Button : Control
{
    public string Text { get; set; }

    public Color? BackgroundColor { get; set; }
    public Color? BorderColor { get; set; }

    public int? WidthOverride { get; set; }
    public int? HeightOverride { get; set; }

    protected Vector2 FontSize { get; set; }

    public Button(string text, IControlContainer container, float x, float y) : base(container, x, y)
        => Text = text;

    public override Vector2 CalculateSize()
    {
        FontSize = MeasureString(Text.Length == 0 ? "X" : Text);
        return BackgroundDrawingMode switch
        {
            BackgroundDrawingMode.Basic => new(WidthOverride ?? FontSize.X + 5, HeightOverride ?? FontSize.Y + 2),
            BackgroundDrawingMode.Texture => Theme.CalculateNewSize(new(WidthOverride ?? FontSize.X, HeightOverride ?? FontSize.Y + 2)),
            _ => new(WidthOverride ?? FontSize.X, HeightOverride ?? FontSize.Y),
        };
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
    {
        switch (BackgroundDrawingMode)
        {
            case BackgroundDrawingMode.Basic:
                DrawBasicButton(spriteBatch, parentOffset,
                                    BackgroundColor ?? Theme.ResolveBackgroundColor(UpdateParameters, Enabled),
                                    BorderColor ?? Theme.ControlBorderColor,
                                    Theme.ResolveTextColor(UpdateParameters, Enabled, false));
                break;
            case BackgroundDrawingMode.Texture:
                DrawTextureButton(spriteBatch, parentOffset, Theme.ResolveTextColor(UpdateParameters, Enabled, false));
                break;
            default:
                spriteBatch.DrawString(Font, Text, Position + parentOffset, Theme.ResolveTextColor(UpdateParameters, Enabled, false));
                break;
        }
    }
    
    protected void DrawTextureButton(SpriteBatch spriteBatch, Vector2 drawOffset, Color textColor)
    {
        var size = CalculateSize();
        Theme.DrawWindow(spriteBatch, Position + drawOffset, size, BackgroundDrawingMode.Texture);
        spriteBatch.DrawString(Font, Text, (size / 2 - FontSize / 2) + Position + drawOffset, textColor);
    }

    protected void DrawBasicButton(SpriteBatch spriteBatch, Vector2 drawOffset, Color backgroundColor, Color borderColor, Color textColor)
    {
        var (x, y) = Position + drawOffset;
        var (w, h) = CalculateSize();
        spriteBatch.FillRectangle(x, y, w, h, backgroundColor);
        spriteBatch.DrawRectangle(x, y, w, h, borderColor);
        spriteBatch.DrawString(Font, Text, new(x + 3, y + 1), textColor);
    }
}
