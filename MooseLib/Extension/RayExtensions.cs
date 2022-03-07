namespace Merthsoft.Moose.MooseEngine.Extension;
public static class RayExtensions
{
    public static IEnumerable<Vector2> CastRayAround(this Vector2 start, int radius, float radiansDelta = .01f, float stepDelta = .5f)
    {
        var (startX, startY) = start;
        for (var d = 0f; d < MathF.PI * 2; d += radiansDelta)
            for (var delta = 1f; delta < radius; delta += stepDelta)
                yield return new(
                    (startX + delta * MathF.Cos(d)).Round(),
                    (startY + delta * MathF.Sin(d)).Round()
                );
    }

    public static IEnumerable<Vector2> CastRay(this Vector2 start, Vector2 end, bool includeStart, bool includeEnd, bool fillCorners = false, bool extend = false)
    {
        var (x1, y1) = (end.X, end.Y);
        var (x2, y2) = (start.X, start.Y);

        var deltaX = (int)Math.Abs(x1 - x2);
        var deltaZ = (int)Math.Abs(y1 - y2);

        if (deltaX == 0 && deltaZ == 0)
            yield break;

        var stepX = x2 < x1 ? 1 : -1;
        var stepZ = y2 < y1 ? 1 : -1;

        var err = deltaX - deltaZ;

        while (true)
        {
            var current = new Vector2(x2, y2);
            if ((current == start && includeStart) ||
                (current == end && includeEnd) ||
                (current != start && current != end))
                yield return current;

            if (!extend && x2 == x1 && y2 == y1)
                break;

            var e2 = 2 * err;

            if (e2 > -deltaZ)
            {
                err -= deltaZ;
                x2 += stepX;
            }


            if (fillCorners)
                yield return new(x2, y2);

            if (!extend && x2 == x1 && y2 == y1)
                break;

            if (e2 < deltaX)
            {
                err += deltaX;
                y2 += stepZ;
            }
        }
    }

    public static IEnumerable<Point> CastRay(this Point start, Point end, bool includeStart, bool includeEnd)
    {
        var xIncrement = (end.X > start.X) ? 1 : -1;
        var yIncrement = (end.Y > start.Y) ? 1 : -1;

        var delta = (start - end).Abs();
        var error = delta.X - delta.Y;
        var errorCorrect = (X: delta.X * 2, Y: delta.Y * 2);

        var current = start;
        while (true)
        {
            if ((current == start && includeStart) ||
                (current == end && includeEnd) ||
                (current != start && current != end))
                yield return current;

            if (current == end)
                yield break;

            if (error > 0)
            {
                current = new(current.X + xIncrement, current.Y);
                error -= errorCorrect.Y;
            }
            else if (error < 0)
            {
                current = new(current.X, current.Y + yIncrement);
                error += errorCorrect.X;
            }
            else
                current = new(current.X + xIncrement, current.Y + yIncrement);
        }
    }
}
