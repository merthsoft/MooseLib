using System.Text;

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
        => $"{char.ToUpper(s[0])}{new string(s.Skip(1).ToArray())}";

    public static string InsertSpacesBeforeCapitalLetters(this string s, char replacementChar = ' ')
    {
        var sb = new StringBuilder(s[0].ToString());
        foreach (var c in s.Skip(1))
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
}
