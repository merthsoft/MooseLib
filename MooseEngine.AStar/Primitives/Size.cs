namespace Merthsoft.Moose.MooseEngine.PathFinding.Primitives;

public struct Size : IEquatable<Size>
{
    public Size(Distance width, Distance height)
    {
        Width = width;
        Height = height;
    }

    public Distance Width { get; }

    public Distance Height { get; }

    public static bool operator ==(Size a, Size b)
        => a.Equals(b);

    public static bool operator !=(Size a, Size b)
        => !a.Equals(b);

    public override string ToString() => $"(width: {Width}, height: {Height})";

    public override bool Equals(object obj) => obj is Size Size && Equals(Size);

    public bool Equals(Size other) => Width == other.Width && Height == other.Height;

    public override int GetHashCode() => -1609761766 + Width.GetHashCode() + Height.GetHashCode();
}
