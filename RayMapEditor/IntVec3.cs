namespace RayMapEditor;
public struct IntVec2 : IEquatable<IntVec2>
{
    public int x;
    public int y;

    public IntVec2(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public static IntVec2 Zero => new IntVec2(0, 0);

    public static IntVec2 North => new IntVec2(0, 1);

    public static IntVec2 East => new IntVec2(1, 0);

    public static IntVec2 South => new IntVec2(0, -1);

    public static IntVec2 West => new IntVec2(-1, 0);

    public static IntVec2 NorthWest => new IntVec2(-1, 1);

    public static IntVec2 NorthEast => new IntVec2(1, 1);

    public static IntVec2 SouthWest => new IntVec2(-1, -1);

    public static IntVec2 SouthEast => new IntVec2(1, -1);

    public static IntVec2 Invalid => new IntVec2(-1000, -1000);

    public static IntVec2 operator +(IntVec2 a, IntVec2 b) => new IntVec2(a.x + b.x, a.y + b.y);

    public static IntVec2 operator -(IntVec2 a, IntVec2 b) => new IntVec2(a.x - b.x, a.y - b.y);

    public static IntVec2 operator *(IntVec2 a, int i) => new IntVec2(a.x * i, a.y * i);

    public static bool operator ==(IntVec2 a, IntVec2 b) => a.x == b.x && a.y == b.y;

    public static bool operator !=(IntVec2 a, IntVec2 b) => a.x != b.x || a.y != b.y;

    public override bool Equals(object obj) => obj is IntVec2 && this.Equals((IntVec2)obj);

    public bool Equals(IntVec2 other) => this.x == other.x && this.y == other.y;

    public override string ToString()
        => $"({x}, {y})";

    public override int GetHashCode()
        => ToString().GetHashCode();
}
