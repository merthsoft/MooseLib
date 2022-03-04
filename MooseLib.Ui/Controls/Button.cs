namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Button : Control
{
    public string Text { get; set; }

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

    public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        switch (BackgroundDrawingMode)
        {
            case BackgroundDrawingMode.Basic:
                DrawBasicButton(spriteBatch, drawOffset);
                break;
            case BackgroundDrawingMode.Texture:
                DrawTextureButton(spriteBatch, drawOffset);
                break;
            default:
                DrawEmptyButton(spriteBatch, drawOffset);
                break;
        }
    }

    protected virtual void PreLabelDraw(SpriteBatch spriteBatch, Vector2 drawOffset) { }

    protected virtual void DrawEmptyButton(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        PreLabelDraw(spriteBatch, drawOffset);
        spriteBatch.DrawString(Font, Text, Position + drawOffset, ResolvedTextColor);
    }

    protected virtual void DrawTextureButton(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        var size = CalculateSize();
        Theme.DrawWindow(spriteBatch, Position + drawOffset, size, BackgroundDrawingMode.Texture);
        PreLabelDraw(spriteBatch, drawOffset);
        spriteBatch.DrawString(Font, Text, (size / 2 - FontSize / 2) + Position + drawOffset, ResolvedTextColor);
    }

    protected virtual void DrawBasicButton(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        var (x, y) = Position + drawOffset;
        var (w, h) = CalculateSize();
        spriteBatch.FillRectangle(x, y, w, h, ResolvedBackgroundColor);
        spriteBatch.DrawRectangle(x, y, w, h, ResolvedBorderColor);
        PreLabelDraw(spriteBatch, drawOffset);
        spriteBatch.DrawString(Font, Text, new(x + 3, y + 1), ResolvedTextColor);
    }
}
