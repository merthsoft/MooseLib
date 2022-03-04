namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class TextureButton : Button
{
    public Texture2D Texture { get; set; }
    public Rectangle? SourceRectangle { get; set; }

    public TextureButton(Texture2D texture, IControlContainer container, float x, float y) : base("", container, x, y)
        => Texture = texture;

    public override Vector2 CalculateSize()
    {
        var (w, h) = base.CalculateSize();
        if (w < (SourceRectangle?.Width ?? Texture.Width))
            w = SourceRectangle?.Width ?? Texture.Width;
        if (h < (SourceRectangle?.Height ?? Texture.Height))
            h = SourceRectangle?.Height ?? Texture.Height;
        return new(w, h);
    }

    protected override void PreLabelDraw(SpriteBatch spriteBatch, Vector2 drawOffset)
        => spriteBatch.Draw(Texture, Position + drawOffset + new Vector2(2f, 2f), SourceRectangle, Color.White);
}
