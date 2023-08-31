namespace Merthsoft.Moose.MooseEngine.PathFinding.Primitives;

public struct GridSize : IEquatable<GridSize>
{
    public GridSize(int columns, int rows)
    {
        Columns = columns;
        Rows = rows;
    }

    public int Columns { get; }

    public int Rows { get; }

    public static bool operator ==(GridSize a, GridSize b)
        => a.Equals(b);

    public static bool operator !=(GridSize a, GridSize b)
        => !a.Equals(b);

    public override string ToString() => $"(columns: {Columns}, rows: {Rows})";

    public override bool Equals(object obj) => obj is GridSize GridSize && Equals(GridSize);

    public bool Equals(GridSize other) => Columns == other.Columns && Rows == other.Rows;

    public override int GetHashCode() => -1609761766 + Columns.GetHashCode() + Rows.GetHashCode();

    public void Deconstruct(out int Columns, out int Rows) => (Columns, Rows) = (this.Columns, this.Rows);
}
