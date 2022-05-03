namespace RayMapEditor;

public static class Primitives
{
    /// <summary>
    /// Swaps two values.
    /// </summary>
    /// <typeparam name="T">The type of the items we're swapping.</typeparam>
    /// <param name="item1">The first item.</param>
    /// <param name="item2">The second item.</param>
    private static void swap<T>(ref T item1, ref T item2)
    {
        var temp = item1;
        item1 = item2;
        item2 = temp;
    }

    private static IEnumerable<IntVec2> FillCorners(this IEnumerable<IntVec2> shape, IntVec2 center)
    {
        foreach (var cell in shape)
        {
            yield return cell;
            if (cell.x == center.x || cell.y == center.y)
                continue;
            var quadrant = cell.x < center.x && cell.y > center.y ? 0 :
                           cell.x < center.x && cell.y < center.y ? 1 :
                           cell.x > center.x && cell.y < center.y ? 2 :
                           cell.x > center.x && cell.y > center.y ? 3 : throw new Exception($"{cell.x},{cell.y} : {center.x},{center.y}");
            switch (quadrant)
            {
                case 0:
                    if (shape.Contains(cell + IntVec2.NorthEast) && !shape.Contains(cell + IntVec2.East) && !shape.Contains(cell + IntVec2.North))
                        yield return cell + IntVec2.North;
                    break;
                case 1:
                    if (shape.Contains(cell + IntVec2.SouthEast) && !shape.Contains(cell + IntVec2.East) && !shape.Contains(cell + IntVec2.South))
                        yield return cell + IntVec2.South;
                    break;
                case 2:
                    if (shape.Contains(cell + IntVec2.SouthWest) && !shape.Contains(cell + IntVec2.West) && !shape.Contains(cell + IntVec2.South))
                        yield return cell + IntVec2.South;
                    break;
                case 3:
                    if (shape.Contains(cell + IntVec2.NorthWest) && !shape.Contains(cell + IntVec2.West) && !shape.Contains(cell + IntVec2.North))
                        yield return cell + IntVec2.North;
                    break;
            }
        }
    }

    public static IEnumerable<IntVec2> Line(int x1, int z1, int x2, int z2, int thickness, bool fillCorners)
    {
        var ret = new HashSet<IntVec2> {
            new IntVec2(x1, z1),
            new IntVec2(x2, z2)
        };

        var deltaX = Math.Abs(x1 - x2);
        var deltaZ = Math.Abs(z1 - z2);
        var stepX = x2 < x1 ? 1 : -1;
        var stepZ = z2 < z1 ? 1 : -1;

        var err = deltaX - deltaZ;

        while (true)
        {
            ret.AddRange(PlotPoint(x2, z2, thickness));
            if (x2 == x1 && z2 == z1)
                break;
            var e2 = 2 * err;

            if (e2 > -deltaZ)
            {
                err -= deltaZ;
                x2 += stepX;
            }

            if (x2 == x1 && z2 == z1)
                break;
            if (fillCorners && thickness == 1)
                ret.Add(new IntVec2(x2, z2));

            if (e2 < deltaX)
            {
                err += deltaX;
                z2 += stepZ;
            }
        }

        return ret;
    }

    private static IEnumerable<IntVec2> PlotPoint(int x, int z, int thickness)
    {
        if (thickness == 1)
            return new[] { new IntVec2(x, z) };
        var negativeReducer = thickness < 0 ? -thickness % 2 == 0 ? 1 : 0 : 0;

        var positiveReducer = thickness > 0 ? thickness % 2 == 0 ? 1 : 0 : 0;
        var step = Math.Abs(thickness) / 2;
        var corner1 = new IntVec2(x - (step - negativeReducer), z - (step - negativeReducer));
        var corner2 = new IntVec2(x + step - positiveReducer, z + step - positiveReducer);
        return Rectangle(corner1.x, corner1.y, corner2.x, corner2.y, true, 0, 1);
    }

    private static IEnumerable<int> Range(int start, int count, bool direction = true)
    {
        if (count < 0)
        {
            count = -count;
            direction = !direction;
        }

        for (var i = 0; i < count; i++)
            yield return start + (direction ? i : -i);
    }

    public static IEnumerable<IntVec2> HorizontalLine(int x1, int x2, int z, int thickness, bool direction)
    {
        if (x1 > x2)
            swap(ref x1, ref x2);
        return Range(x1, x2 - x1 + 1).SelectMany(x => Range(0, thickness, direction).Select(t => new IntVec2(x, z + t)));
    }

    public static IEnumerable<IntVec2> VerticalLine(int x, int z1, int z2, int thickness, bool direction)
    {
        if (z1 > z2)
            swap(ref z1, ref z2);
        return Range(z1, z2 - z1 + 1).SelectMany(z => Range(0, thickness, direction).Select(t => new IntVec2(x + t, z)));
    }

    public static IEnumerable<IntVec2> Line(IntVec2 vert1, IntVec2 vert2, int thickness, bool fillCorners) =>
        Line(vert1.x, vert1.y, vert2.x, vert2.y, thickness, fillCorners);

    public static IEnumerable<IntVec2> Rectangle(int x1, int z1, int x2, int z2, bool fill, int rotation, int thickness)
    {
        var ret = new HashSet<IntVec2>();

        if (x1 > x2)
            swap(ref x1, ref x2);

        if (z1 > z2)
            swap(ref z1, ref z2);

        if (thickness < 0)
        {
            x1 += thickness + 1;
            z1 += thickness + 1;
            x2 -= thickness + 1;
            z2 -= thickness + 1;

            thickness = -thickness;
        }

        if (rotation == 0)
        {
            if (fill)
                for (var x = x1; x <= x2; x++)
                    ret.AddRange(VerticalLine(x, z1, z2, 1, false));
            else
            {
                ret.AddRange(HorizontalLine(x1, x2, z1, thickness, true));
                ret.AddRange(HorizontalLine(x1, x2, z2, thickness, false));
                ret.AddRange(VerticalLine(x1, z1, z2, thickness, true));
                ret.AddRange(VerticalLine(x2, z1, z2, thickness, false));
            }
            return ret;
        }
        else
        {
            if (x1 > x2)
                swap(ref x1, ref x2);

            if (z1 > z2)
                swap(ref z1, ref z2);

            var hr = (x2 - x1) / 2;
            var kr = (z2 - z1) / 2;

            var A = new IntVec2(x1, z1 + kr);
            var B = new IntVec2(x1 + hr, z1);
            var C = new IntVec2(x2, z1 + kr);
            var D = new IntVec2(x1 + hr, z2);

            ret.AddRange(Line(A, B, thickness, false));
            ret.AddRange(Line(C, B, thickness, false));
            ret.AddRange(Line(A, D, thickness, false));
            ret.AddRange(Line(C, D, thickness, false));

            if (fill)
                return Fill(ret);
            else
                return ret;
        }
    }

    public static IEnumerable<IntVec2> Octagon(int sx, int sy, int sz, int tx, int ty, int tz, bool fill, int rotation)
    {
        var ret = new HashSet<IntVec2>();
        return ret;
    }

    public static IEnumerable<IntVec2> Pentagon(int sx, int sy, int sz, int tx, int ty, int tz, bool fill, int rotation, int thickness)
    {
        var ret = new HashSet<IntVec2>();

        IntVec2 A, B, C, D, E;
        var width = tx - sx;
        var height = tz - sz;

        var middleX = width / 2 + sx;
        var middleZ = height / 2 + sz;

        var thirdWidth = width / 3;
        var thirdHeight = height / 3;

        switch (rotation)
        {
            case 0:
            default:
                A = new IntVec2(middleX, sz);
                B = new IntVec2(sx, middleZ);
                C = new IntVec2(tx, middleZ);
                D = new IntVec2(sx + thirdWidth, tz);
                E = new IntVec2(tx - thirdWidth, tz);
                break;
            case 1:
                A = new IntVec2(sx, middleZ);
                B = new IntVec2(middleX, sz);
                C = new IntVec2(middleX, tz);
                D = new IntVec2(tx, sz + thirdHeight);
                E = new IntVec2(tx, tz - thirdHeight);
                break;
        }

        ret.AddRange(Line(A, B, thickness, false));
        ret.AddRange(Line(A, C, thickness, false));
        ret.AddRange(Line(B, D, thickness, false));
        ret.AddRange(Line(C, E, thickness, false));
        ret.AddRange(Line(D, E, thickness, false));

        if (fill)
            return Fill(ret);
        else
            return ret;
    }

    public static IEnumerable<IntVec2> Hexagon(int sx, int sy, int sz, int tx, int ty, int tz, bool fill, int rotation, int thickness)
    {
        if (tx < sx)
            swap(ref sx, ref tx);
        if (tz < sz)
            swap(ref sz, ref tz);
        IntVec2 A, B, C, D, E, F;

        if (rotation == 0)
        {
            var w = tx - sx;
            var h = tz - sz;
            var mz = h / 2 + sz;
            var wt = w / 4;

            A = new IntVec2(sx, mz);
            B = new IntVec2(sx + wt, sz);
            C = new IntVec2(sx + wt, tz);
            D = new IntVec2(tx - wt, sz);
            E = new IntVec2(tx - wt, tz);
            F = new IntVec2(tx, mz);
        }
        else
        {
            var w = tx - sx;
            var h = tz - sz;
            var mx = w / 2 + sx;
            var ht = h / 4;

            A = new IntVec2(mx, sz);
            B = new IntVec2(tx, sz + ht);
            C = new IntVec2(sx, sz + ht);
            D = new IntVec2(tx, tz - ht);
            E = new IntVec2(sx, tz - ht);
            F = new IntVec2(mx, tz);
        }

        var ret = new HashSet<IntVec2>();

        ret.AddRange(Line(A, B, thickness, false));
        ret.AddRange(Line(B, D, thickness, false));
        ret.AddRange(Line(F, D, thickness, false));
        ret.AddRange(Line(A, C, thickness, false));
        ret.AddRange(Line(C, E, thickness, false));
        ret.AddRange(Line(F, E, thickness, false));
        // ret.AddRange(new[] { A, B, C, D, E, F });

        if (fill)
            return Fill(ret);
        else
            return ret;
    }

    private static void AddRange(this HashSet<IntVec2> vectors, IEnumerable<IntVec2> newVectors)
    {
        foreach (var vec in newVectors)
            vectors.Add(vec);
    }

    /// <summary>
    /// Draws an ellipse to the sprite.
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="z1"></param>
    /// <param name="x2"></param>
    /// <param name="z2"></param>
    /// <param name="fill">True to fill the ellipse.</param>
    public static IEnumerable<IntVec2> Ellipse(int x1, int z1, int x2, int z2, bool fill, int thickness, bool fillCorners)
    {
        if (x2 < x1)
            swap(ref x1, ref x2);
        if (z2 < z1)
            swap(ref z1, ref z2);
        var hr = (x2 - x1) / 2;
        var kr = (z2 - z1) / 2;
        var h = x1 + hr;
        var k = z1 + kr;

        return RadialEllipse(h, k, hr, kr, fill, thickness, fillCorners);
    }

    private static void incrementX(ref int x, ref int dxt, ref int d2xt, ref int t)
    {
        x++;
        dxt += d2xt;
        t += dxt;
    }

    private static void incrementY(ref int y, ref int dyt, ref int d2yt, ref int t)
    {
        y--;
        dyt += d2yt;
        t += dyt;
    }

    /// <summary>
    /// Draws a filled ellipse to the sprite.
    /// </summary>
    /// <remarks>Taken from http://enchantia.com/graphapp/doc/tech/ellipses.html.</remarks>
    /// <param name="x">The center point X coordinate.</param>
    /// <param name="z">The center point Z coordinate.</param>
    /// <param name="xRadius">The x radius.</param>
    /// <param name="zRadius">The z radius.</param>
    /// <param name="fill">True to fill the ellipse.</param>
    public static IEnumerable<IntVec2> RadialEllipse(int x, int z, int xRadius, int zRadius, bool fill, int thickness, bool fillCorners)
    {
        var ret = new HashSet<IntVec2>();

        if (thickness != 1)
        {
            foreach (var i in Enumerable.Range(0, thickness))
                ret.AddRange(RadialEllipse(x, z, xRadius - i, zRadius - i, fill, 1, true));

            return ret;
        }

        var plotX = 0;
        var plotZ = zRadius;

        var xRadiusSquared = xRadius * xRadius;
        var zRadiusSquared = zRadius * zRadius;
        var crit1 = -(xRadiusSquared / 4 + xRadius % 2 + zRadiusSquared);
        var crit2 = -(zRadiusSquared / 4 + zRadius % 2 + xRadiusSquared);
        var crit3 = -(zRadiusSquared / 4 + zRadius % 2);

        var t = -xRadiusSquared * plotZ;
        var dxt = 2 * zRadiusSquared * plotX;
        var dzt = -2 * xRadiusSquared * plotZ;
        var d2xt = 2 * zRadiusSquared;
        var d2zt = 2 * xRadiusSquared;

        while (plotZ >= 0 && plotX <= xRadius)
        {
            circlePlot(x, z, ret, plotX, plotZ, fill, thickness);

            if (t + zRadiusSquared * plotX <= crit1 || t + xRadiusSquared * plotZ <= crit3)
                incrementX(ref plotX, ref dxt, ref d2xt, ref t);
            else if (t - xRadiusSquared * plotZ > crit2)
                incrementY(ref plotZ, ref dzt, ref d2zt, ref t);
            else
            {
                incrementX(ref plotX, ref dxt, ref d2xt, ref t);
                if (fillCorners)
                    circlePlot(x, z, ret, plotX, plotZ, fill, 1);
                incrementY(ref plotZ, ref dzt, ref d2zt, ref t);
            }
        }

        return ret;
    }

    private static void circlePlot(int x, int z, HashSet<IntVec2> ret, int plotX, int plotZ, bool fill, int thickness)
    {
        var center = new IntVec2(x, z);
        ret.AddRange(plotOrLine(center, new IntVec2(x + plotX, z + plotZ), fill, thickness));
        if (plotX != 0 || plotZ != 0)
            ret.AddRange(plotOrLine(center, new IntVec2(x - plotX, z - plotZ), fill, thickness));

        if (plotX != 0 && plotZ != 0)
        {
            ret.AddRange(plotOrLine(center, new IntVec2(x + plotX, z - plotZ), fill, thickness));
            ret.AddRange(plotOrLine(center, new IntVec2(x - plotX, z + plotZ), fill, thickness));
        }
    }

    private static IEnumerable<IntVec2> plotOrLine(IntVec2 point1, IntVec2 point2, bool line, int thickness)
    {
        if (line)
            return Line(point1, point2, 1, false);
        else
            return PlotPoint(point2.x, point2.y, thickness);
    }

    public static IEnumerable<IntVec2> Circle(IntVec2 s, IntVec2 t, bool filled, int thickness, bool fillCorners)
    {
        var x1 = s.x;
        var z1 = s.y;
        var x2 = t.x;
        var z2 = t.y;

        if (x2 < x1)
            swap(ref x1, ref x2);
        if (z2 < z1)
            swap(ref z1, ref z2);
        var r = Math.Max(x2 - x1, z2 - z1);
        return Circle(s.x, s.y, r, filled, thickness, fillCorners);
    }

    public static IEnumerable<IntVec2> Circle(IntVec2 center, int r, bool fill, int thickness, bool fillCorners) =>
        Circle(center.x, center.y, r, fill, thickness, fillCorners);

    /// <summary>
    /// Draws a circle to the sprite.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="r"></param>
    /// <param name="fill">True to fill the circle.</param>
    public static IEnumerable<IntVec2> Circle(int x, int z, int r, bool fill, int thickness, bool fillCorners) =>
        RadialEllipse(x, z, r, r, fill, thickness, fillCorners);

    public static IEnumerable<IntVec2> Fill(IEnumerable<IntVec2> outLine)
    {
        var ret = new HashSet<IntVec2>();
        foreach (var lineGroup in outLine.GroupBy(vec => vec.y))
            if (lineGroup.Count() == 1)
                ret.Add(lineGroup.First());
            else
            {
                var sorted = lineGroup.OrderBy(v => v.x);
                var point1 = sorted.First();
                var point2 = sorted.Last();
                ret.AddRange(HorizontalLine(point1.x, point2.x, lineGroup.Key, 1, true));
            }

        return ret;
    }
}
