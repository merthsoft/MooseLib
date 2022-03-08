namespace Merthsoft.Moose.MooseEngine.Extension;
public static class RectangleExtensions
{

    public static RectangleF Move(this RectangleF rect, Vector2 delta)
        => new(rect.X + delta.X, rect.Y + delta.Y, rect.Width, rect.Height);

    public static bool Intersects(this RectangleF rect, float x, float y)
        => x >= rect.X
            && x <= rect.X + rect.Width
        && y >= rect.Y
            && y <= rect.Y + rect.Height;

    public static bool Intersects(this Rectangle rect, int x, int y)
        => x >= rect.X
            && x <= rect.X + rect.Width
        && y >= rect.Y
            && y <= rect.Y + rect.Height;

    public static bool Intersects(this RectangleF rect, Vector2 point)
        => point.X >= rect.X
            && point.X <= rect.X + rect.Width
        && point.Y >= rect.Y
            && point.Y <= rect.Y + rect.Height;

    public static bool Intersects(this Rectangle rect, Point point)
        => point.X >= rect.X
            && point.X <= rect.X + rect.Width
        && point.Y >= rect.Y
            && point.Y <= rect.Y + rect.Height;

    public static bool Contains(this RectangleF rect, Rectangle otherRect)
        => rect.Contains(otherRect.Location) && rect.Contains(otherRect.Location + otherRect.Size);
}
