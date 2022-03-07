namespace Merthsoft.Moose.MooseEngine.Extension;
public static class MathExtensions
{
    public static bool InBounds(this Vector2 pos, int width, int height)
        => pos.X >= 0 && pos.Y >= 0 && pos.X < width && pos.Y < height;

    public static Point Abs(this Point p)
        => new(Math.Abs(p.X), Math.Abs(p.Y));

    public static Vector2 Round(this Vector2 vector, int digits)
        => new(vector.X.Round(digits), vector.Y.Round(digits));
    
    public static Vector2 GetFloor(this Vector2 vector)
        => new((int)vector.X, (int)vector.Y);

    public static (float, float) Floor(this (float X, float Y) vec)
        => new(MathF.Floor(vec.X), MathF.Floor(vec.Y));

    public static int Floor(this float f)
        => (int)MathF.Floor(f);

    public static int Ceiling(this float f)
        => (int)MathF.Ceiling(f);

    public static int Ceiling(this decimal f)
        => (int)Math.Ceiling(f);

    public static float Round(this float f, int digits)
        => MathF.Round(f, digits);

    public static int Round(this float f)
        => (int)MathF.Round(f, 0);

    public static long Sum(this IEnumerable<byte> set)
        => set.Sum(b => (long)b);

    public static Vector2 RotateAround(this Vector2 point, Vector2 center, float angleInDegrees)
    {
        var rad = angleInDegrees * (MathF.PI / 180);
        var s = MathF.Sin(rad);
        var c = MathF.Cos(rad);

        // translate point back to origin:
        var oldX = point.X - center.X;
        var oldY = point.Y - center.Y;

        // rotate point
        var newX = oldX * c - oldY * s;
        var newY = oldX * s + oldY * c;

        // translate point back:
        return new(newX + center.X, newY + center.Y);
    }

}
