namespace Merthsoft.Moose.MooseEngine.PathFinding;

public struct CellSize : IEquatable<CellSize>
{
    public CellSize(Distance width, Distance height)
    {
        Width = width;
        Height = height;
    }

    public Distance Width { get; }

    public Distance Height { get; }

    public static bool operator ==(CellSize a, CellSize b)
        => a.Equals(b);

    public static bool operator !=(CellSize a, CellSize b)
        => !a.Equals(b);

    public override string ToString() => $"(width: {Width}, height: {Height})";

    public override bool Equals(object obj) => obj is CellSize Size && Equals(Size);

    public bool Equals(CellSize other) => Width == other.Width && Height == other.Height;

    public override int GetHashCode() => -1609761766 + Width.GetHashCode() + Height.GetHashCode();

    public void Deconstruct(out float Width, out float Height) => (Width, Height) = (this.Width.Meters, this.Height.Meters);
}
