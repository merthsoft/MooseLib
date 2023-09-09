namespace Merthsoft.Moose.MooseEngine.PathFinding;

public class Edge : IEquatable<Edge>
{
    private Velocity traversalVelocity;

    public Edge(Node start, Node end, Velocity traversalVelocity)
    {
        Start = start;
        End = end;

        Distance = Distance.BeweenPositions(start.Position, end.Position);
        TraversalVelocity = traversalVelocity;
    }

    public Velocity TraversalVelocity
    {
        get => traversalVelocity;
        set
        {
            traversalVelocity = value;
            TraversalDuration = Distance / value;
        }
    }

    public Duration TraversalDuration { get; private set; }

    public Distance Distance { get; }

    public Node Start { get; }

    public Node End { get; }

    public bool IsConnected { get; set; } = false;

    public static bool operator ==(Edge? lhs, Edge? rhs)
        => lhs is null && rhs is null ? true : lhs?.Equals(rhs) ?? false;

    public static bool operator !=(Edge? lhs, Edge? rhs)
        => !(lhs == rhs);

    public bool Equals(Edge? other)
        => Start == other?.Start && End == other?.End;

    public override string ToString() => $"{Start} {(IsConnected ? "->" : "-/>")} {End} @ {TraversalVelocity}";
}
