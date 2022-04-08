using System.Collections;

namespace Merthsoft.Moose.Rays;

public record RayTexture : IEnumerable<ColorStrip>
{
    public static RayTexture Empty { get; } = new(0, 0);

    public int Width { get; set; }
    public int Height { get; set; }

    public List<ColorStrip> ColorStrips { get; } = new();

    public ColorStrip this[int col]
        => ColorStrips[col];

    public uint this[int x, int y]
    {
        get => ColorStrips[x][y];
        set => ColorStrips[x][y] = value;
    }

    protected RayTexture()
        => Width = Height = 0;
    public RayTexture(int w, int h)
    {
        Width = w;
        Height = h;
    }

    public RayTexture(int w, int h, IEnumerable<ColorStrip> strips)
        : this(w, h)
        => ColorStrips.AddRange(strips);

    private ColorStrip NewStrip()
    {
        var ret = new ColorStrip();
        ColorStrips.Add(ret);
        return ret;
    }

    public void Add(uint color)
    {
        var strip = ColorStrips.LastOrDefault();
        if (strip == null || strip.Count == Height)
            strip = NewStrip();
        strip.Add(color);
    }

    public RayTexture Zip(RayTexture other)
        => new(Width, Height, ColorStrips.Zip(other.ColorStrips, (cs1, cs2) => cs1.Zip(cs2)));

    public RayTexture GenerateNorthWall()
    {
        var ret = new RayTexture(Width, Height);
        foreach (var strip in ColorStrips)
            foreach (var color in strip)
                ret.Add(color.Blend(GraphicsExtensions.Black));

        return ret;
    }

    #region IEnumerable<ColorStrip>
    public IEnumerator<ColorStrip> GetEnumerator() => ((IEnumerable<ColorStrip>)ColorStrips).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ColorStrips).GetEnumerator();
    #endregion
}
