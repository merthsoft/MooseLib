namespace Merthsoft.Moose.MooseEngine.Extension;
public static class RayExtensions
{
    public static IEnumerable<Vector2> CastRay(this Vector2 start, float angleRadians, bool includeStart, bool fillCorners = false)
    {
        var (x2, y2) = (start.X, start.Y);
        var x1 = x2 + 10000 * angleRadians.Cos();
        var y1 = y2 + 10000 * angleRadians.Sin();

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
            if ((current == start && includeStart) || current != start)
                yield return current;

            var e2 = 2 * err;

            if (e2 > -deltaZ)
            {
                err -= deltaZ;
                x2 += stepX;
            }


            if (fillCorners)
                yield return new(x2, y2);

            if (e2 < deltaX)
            {
                err += deltaX;
                y2 += stepZ;
            }
        }
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
}
