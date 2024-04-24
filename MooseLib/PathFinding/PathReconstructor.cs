namespace Merthsoft.Moose.MooseEngine.PathFinding;

internal sealed class PathReconstructor
{
    private readonly Dictionary<Node, Edge> CameFrom;

    public PathReconstructor() => CameFrom = [];

    public void SetCameFrom(Node node, Edge via)
        => CameFrom[node] = via;

    public Path ConstructPathTo(Node node, Node goal)
    {
        var current = node;
        var edges = new List<Edge>();

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
