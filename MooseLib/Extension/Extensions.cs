using Roy_T.AStar.Grids;
using System.Text;

namespace Merthsoft.Moose.MooseEngine.Extension;

public static class Extensions
{
    public static bool IsPrintableAscii(this char c)
        => char.IsLetterOrDigit(c)
        || char.IsPunctuation(c)
        || char.IsSymbol(c)
        || c == ' ';

    public static string UpperFirst(this string s)
        => $"{char.ToUpper(s[0])}{new string(s.Skip(1).ToArray())}";

    public static Grid DisconnectWhere(this Grid grid, Func<int, int, bool> func)
    {
        for (var x = 0; x < grid.Columns; x++)
            for (var y = 0; y < grid.Rows; y++)
                if (func(x, y))
                    grid.DisconnectNode(new(x, y));

        return grid;
    }

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
}
