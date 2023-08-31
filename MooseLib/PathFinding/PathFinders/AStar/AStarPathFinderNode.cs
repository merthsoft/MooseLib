namespace Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.AStar;

internal sealed class AStarPathFinderNode : IComparable<AStarPathFinderNode>
{
    public AStarPathFinderNode(Node node, Duration durationSoFar, Duration expectedRemainingTime)
    {
        Node = node;
        DurationSoFar = durationSoFar;
        ExpectedRemainingTime = expectedRemainingTime;
        ExpectedTotalTime = DurationSoFar + ExpectedRemainingTime;
    }

    public Node Node { get; }

    public Duration DurationSoFar { get; }

    public Duration ExpectedRemainingTime { get; }

    public Duration ExpectedTotalTime { get; }

    public int CompareTo(AStarPathFinderNode other) => ExpectedTotalTime.CompareTo(other.ExpectedTotalTime);

    public override string ToString() => $"📍{{{Node.Position.X}, {Node.Position.Y}}}, ⏱~{ExpectedTotalTime}";
}
