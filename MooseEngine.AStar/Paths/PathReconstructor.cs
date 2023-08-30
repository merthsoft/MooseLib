namespace Merthsoft.Moose.MooseEngine.PathFinding.Paths;

internal sealed class PathReconstructor
{
    private readonly Dictionary<INode, IEdge> CameFrom;

    public PathReconstructor() => CameFrom = new Dictionary<INode, IEdge>();

    public void SetCameFrom(INode node, IEdge via)
        => CameFrom[node] = via;

    public Path ConstructPathTo(INode node, INode goal)
    {
        var current = node;
        var edges = new List<IEdge>();

        while (CameFrom.TryGetValue(current, out var via))
        {
            edges.Add(via);
            current = via.Start;
        }

        edges.Reverse();

        var type = node == goal ? PathType.Complete : PathType.ClosestApproach;
        return new Path(type, edges);
    }

    public void Clear() => CameFrom.Clear();
}
