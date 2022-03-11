namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Rect : Control
{
    public Vector2 Size { get; set; }

    public int Thickness { get; set; } = 1;

    public bool DrawBorder { get; set; } = true;
    public bool DrawFill { get; set; } = true;

    public override Vector2 CalculateSize() => Size;

    public Rect(IControlContainer container, float x, float y, float w, float h, int thickness = 1) : base(container, x, y)
    {
        Size = new(w, h);
        Thickness = thickness;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset, GameTime gameTime)
    {
        if (DrawFill)
            spriteBatch.FillRectangle(Position + parentOffset, Size, BackgroundColor ?? Theme.ResolveBackgroundColor(UpdateParameters, true, false));
        if (DrawBorder)
            spriteBatch.DrawRectangle(Position + parentOffset, Size, BorderColor ?? Theme.ControlBorderColor, Thickness);
    }
}
