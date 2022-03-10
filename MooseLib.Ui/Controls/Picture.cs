namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Picture : Control, IDisposable
{
    public Texture2D Texture { get; set; }

    private Rectangle? sourceRectangle;

    public Rectangle SourceRectangle
    {
        get => sourceRectangle ?? new(0, 0, Texture.Width, Texture.Height);
        set => sourceRectangle = value;
    }

    public Vector2 Scale { get; set; } = Vector2.One;

    public float Rotation { get; set; }

    public SpriteEffects SpriteEffects { get; set; }

    public Color Color { get; set; } = Color.White;

    public override Vector2 CalculateSize() => new((SourceRectangle.Width * Scale.X).Ceiling(), (SourceRectangle.Height * Scale.Y).Ceiling());

    public Picture(IControlContainer container, float x, float y, Texture2D texture) : base(container, x, y)
        => Texture = texture;

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        => spriteBatch.Draw(Texture, Position + parentOffset, SourceRectangle, Color, Rotation, Vector2.Zero, Scale, SpriteEffects, 1);
    public void Dispose() { }//((IDisposable)Texture).Dispose();
}
