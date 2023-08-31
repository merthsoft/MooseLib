namespace Merthsoft.Moose.MooseEngine.PathFinding.Paths;

public sealed class Path
{
    public Path(PathType type, IReadOnlyList<Edge> edges)
    {
        Type = type;
        Edges = edges;

        for (var i = 0; i < Edges.Count; i++)
        {
            Duration += Edges[i].TraversalDuration;
            Distance += Edges[i].Distance;
        }
    }

    public PathType Type { get; }

    public Duration Duration { get; }

    public IReadOnlyList<Edge> Edges { get; }

    public Distance Distance { get; }
}
