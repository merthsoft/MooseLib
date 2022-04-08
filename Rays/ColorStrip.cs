using System.Collections;

namespace Merthsoft.Moose.Rays;

public class ColorStrip : IEnumerable<uint>
{
    public List<uint> Strip { get; set; }

    public int Count => Strip.Count;

    public uint this[int index]
    {
        get => Strip[index];
        set => Strip[index] = value;
    }

    public ColorStrip()
        => Strip = new List<uint>();

    public ColorStrip(IEnumerable<uint> strip)
        => Strip = strip.ToList();

    public void Add(uint argb)
        => Strip.Add(argb);

    public ColorStrip Zip(ColorStrip strip)
        => new(Strip.Zip(strip, (s1, s2) => s1.IsWhiteTransparent() ? s2 : s1));

    public void Resize(int w)
        => Strip.Resize(w, GraphicsExtensions.Black);

    #region IEnumerable<uint>
    public IEnumerator<uint> GetEnumerator() => ((IEnumerable<uint>)Strip).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Strip).GetEnumerator();
    #endregion
}
