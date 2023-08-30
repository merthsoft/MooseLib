namespace Merthsoft.Moose.MooseEngine.PathFinding.Graphs;

public sealed class Edge : IEdge
{
    private Velocity traversalVelocity;

    public Edge(INode start, INode end, Velocity traversalVelocity)
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

    public INode Start { get; }

    public INode End { get; }

    public bool IsConnected { get; set; } = false;
    public bool IsIncoming { get; set; } = false;

    public override string ToString() => $"{Start} {(IsConnected ? "->" : "-/>")} {End} @ {TraversalVelocity}";
}
