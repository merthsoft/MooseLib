namespace Merthsoft.Moose.Dungeon;

public record FallingText
{
    public string Text { get; set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public SpriteEffects Effects { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;
    public Color Color { get; set; } = Color.White;
    public int Age { get; set; }

    public bool Done { get; set; } = false;

    public FallingText(string text, Vector2 position, Color? color = null)
    {
        Text = text;
        Position = position;
        Color = color ?? Color;
    }
}
