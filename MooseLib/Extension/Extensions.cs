using System.Text;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Merthsoft.Moose.MooseEngine.Extension;

public static class Extensions
{ 
    public static string JoinString<T>(this IEnumerable<T> set, string? separator, Func<T, string>? selector = null)
        => selector == null
            ? string.Join(separator, set)
            : string.Join(separator, set.Select(selector));

    public static bool IsLowercaseLetter(this char c)
        => char.IsLower(c);

    public static bool IsPrintableAscii(this char c)
        => char.IsLetterOrDigit(c)
        || char.IsPunctuation(c)
        || char.IsSymbol(c)
        || c == ' ';

    public static string UpperFirst(this string s)
        => $"{char.ToUpper(s[0])}{s[1..]}";

    public static string InsertSpacesBeforeCapitalLetters(this string s, char replacementChar = ' ')
    {
        var sb = new StringBuilder(s[0].ToString(), s.Length * 2);
        foreach (var c in s[1..])
        {
            if (char.IsUpper(c))
                sb.Append(replacementChar);
            sb.Append(c);
        }
        return sb.ToString();
    }

    public static IEnumerable<Point> SpiralAround(int x, int y)
        => SpiralAround(new Point(x, y));

    public static IEnumerable<Point> SpiralAround(this Point x)
    {
        int stepNum = 0;
        while (true)
            yield return x.FindSpiralStep(stepNum++);
    }

    public static Point FindSpiralStep(this Point p, int n)
    {
        var k = (int)MathF.Ceiling((MathF.Sqrt(n) - 1) / 2);
        var t = 2 * k + 1;
        var m = t * t;
        t = t - 1;

        var (x, y) = p;

        if (n >= m - t)
            return new(k - (m - n) + x, -k + y);
        else
            m = m - t;

        if (n >= m - t)
            return new(-k + x, -k + (m - n) + y);
        else
            m = m - t;

        if (n >= m - t)
            return new(-k + (m - n) + x, k + y);

        return new(k + x, k - (m - n - t) + y);
    }

    public static void ZoomToPoint(this OrthographicCamera camera, float deltaZoom, Vector2 zoomCenter)
    {
        float pastZoom = camera.Zoom;
        camera.ZoomIn(deltaZoom);
        camera.Position += (zoomCenter - camera.Origin - camera.Position) * ((camera.Zoom - pastZoom) / camera.Zoom);
    }

    public static TEnum Next<TEnum>(this TEnum t) where TEnum : struct, Enum
    {   
        var values = Enum.GetValues<TEnum>();
        var index = Array.IndexOf(values, t);
        index = index >= values.Length - 1 ? 0 : index + 1;
        return values[index];
    }
}
