namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Button : Control
{
    public string Text { get; set; }
    
    public Texture2D? Texture { get; set; }
    
    public Rectangle? SourceRectangle { get; set; }
    public Vector2 TextureScale { get; set; } = Vector2.One;

    public int? WidthOverride { get; set; }
    public int? HeightOverride { get; set; }

    protected Vector2 FontSize { get; set; }

    public Vector2 LabelOffset { get; set; }

    public Button(string text, IControlContainer container, float x, float y) : base(container, x, y)
        => Text = text;

    public override Vector2 CalculateSize()
    {
        FontSize = MeasureString(Text.Length == 0 ? "" : Text);
        var (w, h) = BackgroundDrawingMode switch
        {
            BackgroundDrawingMode.Basic => new(WidthOverride ?? FontSize.X + 5, HeightOverride ?? FontSize.Y + 2),
            BackgroundDrawingMode.Texture => Theme.CalculateNewSize(new(WidthOverride ?? FontSize.X, HeightOverride ?? FontSize.Y + 2)),
            _ => new(WidthOverride ?? FontSize.X, HeightOverride ?? FontSize.Y),
        };

        if (Texture == null)
            return new(w, h);

        if (w < (SourceRectangle?.Width ?? Texture.Width) * TextureScale.X)
            w = (SourceRectangle?.Width ?? Texture.Width) * TextureScale.X;
        if (h < (SourceRectangle?.Height ?? Texture.Height) * TextureScale.Y + FontSize.Y)
            h = (SourceRectangle?.Height ?? Texture.Height) * TextureScale.Y + FontSize.Y;
        return Theme.CalculateNewSize(new(w, h));
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset, GameTime gameTime)
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

    protected virtual void PreLabelDraw(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        if (Texture != null)
            spriteBatch.Draw(Texture, Position + drawOffset + new Vector2(2f, 2f), SourceRectangle,
                ResolvedMainColorShift, 0, Vector2.Zero, TextureScale, SpriteEffects.None, 1f);
    }

    protected virtual void DrawEmptyButton(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        PreLabelDraw(spriteBatch, drawOffset);
        spriteBatch.DrawString(Font, Text, Position + drawOffset + LabelOffset, ResolvedTextColor);
    }

    protected virtual void DrawTextureButton(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        var size = CalculateSize();
        var windowSize = new Vector2(size.X, size.Y - FontSize.Y);
        var themeOffset = Theme.DrawWindow(spriteBatch, Position + drawOffset, windowSize, BackgroundDrawingMode.Texture, ResolvedTextColor);
        PreLabelDraw(spriteBatch, drawOffset + themeOffset);
        var position = Position + drawOffset + themeOffset + LabelOffset;
        if (Texture != null)
            position = new(position.X, Position.Y + drawOffset.Y + Theme.CalculateNewSize(size).Y - FontSize.Y + LabelOffset.Y);
        
        spriteBatch.DrawString(Font, Text, position, ResolvedTextColor);
    }

    protected virtual void DrawBasicButton(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        var (x, y) = Position + drawOffset;
        var (w, h) = CalculateSize();
        spriteBatch.FillRectangle(x, y, w, h, ResolvedBackgroundColor);
        spriteBatch.DrawRectangle(x, y, w, h, ResolvedBorderColor);
        PreLabelDraw(spriteBatch, drawOffset);
        spriteBatch.DrawString(Font, Text, new Vector2(x + 3, y + 1) + LabelOffset, ResolvedTextColor);
    }
}
