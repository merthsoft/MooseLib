using Merthsoft.Moose.MooseEngine.PathFinding.Grids;

namespace Merthsoft.Moose.MooseEngine.PathFinding.Paths.FlowFieldPathFinder;

public record FlowNode(int NextX, int NextY, float CostValue)
{
    public bool Valid => NextX != -1 && NextY != -1;
}

public static class FlowField
{
    public struct AdjacentTile<T>
    {
        public int XOffset;
        public int YOffset;
        public T Value;

        internal void Deconstruct(out int xOffset, out int yOffset, out T value)
            => (xOffset, yOffset, value)
             = (XOffset, YOffset, Value);
    }

    public static FlowNode[,] GenerateFlow(Grid grid, Point destination)
    {
        var (w, h) = grid.GridSize;
        var costMap = new float[w, h];
        var visitedMap = new bool[w, h];

        var nodes = new List<(Node current, Edge? previous)>() { (grid.GetNode(destination), null) };
        var index = 0;
        while (index != nodes.Count())
        {
            var tuple = nodes[index++];
            var (node, previous) = tuple;
            var (x, y) = node.PointPosition;
            if (visitedMap[x, y])
                continue;
            visitedMap[x, y] = true;
            if (previous == null)
            {
                costMap[x, y] = 0;
            } else
            {
                var (px, py) = previous.End.PointPosition;
                costMap[x, y] = costMap[px, py] + previous.TraversalVelocity.MetersPerSecond;
                if (!node.Incoming.Any())
                    costMap[x, y] += 10000;
            }
            foreach (var i in node.AllIncoming)
            {
                //if (!i.Start.IsOutgoingConnected(node))
                //    continue;
                var (ix, iy) = i.Start.PointPosition;
                if (!visitedMap[ix, iy])
                    nodes.Add((i.Start, i));
            }
        }

        var convoluted = new FlowNode[w, h];
        for (var x = 0; x < w; x++)
            for (var y = 0; y < h; y++)
            {                   
                if (x == destination.X && y == destination.Y)
                {
                    convoluted[x, y] = new(-1, -1, 0);
                    continue;
                }

                var node = grid.GetNode(x, y);

                var bestNode = node.Outgoing.Where(o => o.End.IsOutgoingConnected(node)).OrderBy(i => costMap[i.End.PointPosition.X, i.End.PointPosition.Y]).FirstOrDefault();
                if (bestNode != null)
                    convoluted[x, y] = new(bestNode.End.PointPosition.X, bestNode.End.PointPosition.Y, costMap[x, y]);
                else
                    convoluted[x, y] = new(-1, -1, costMap[x, y]);
            }

        return convoluted;

        float ValueIfInBounds(int x, int y, float[,] values)
            => InBounds(x, y) ? values[x, y] : float.MaxValue;

        bool InBounds(int x, int y)
            => x >= 0 && x < grid.Rows
            && y >= 0 && y < grid.Columns;
    }
}

public class FlowFieldPathFinder : IPathFinder
{
    Dictionary<Point, FlowNode[,]> flowCache = new();

    public FlowFieldPathFinder()
    {

    }

    public void ClearCache()
        => flowCache.Clear();

    public FlowNode[,] GetFlow(Grid grid, Point endPoint)
        => flowCache[endPoint] = flowCache.GetValueOrDefault(endPoint) ?? FlowField.GenerateFlow(grid, endPoint);

    public Path FindPath(int x1, int y1, int x2, int y2, Grid grid, Velocity? maximumVelocity = null)
    {
        var endPoint = new Point(x2, y2);
        var flow = GetFlow(grid, endPoint);
        var currentNode = grid.GetNode(x1, y1);
        var edges = new HashSet<Edge>();
        var completed = true;
        while (currentNode.PointPosition != endPoint)
        {
            var (x, y) = currentNode.PointPosition;
            var f = flow[x, y];
            if (f.NextX == -1 || f.NextY == -1)
            {
                completed = false;
                break;
            }
            var nextNode = grid.GetNode(f.NextX, f.NextY);
            var edge = nextNode.Incoming.FirstOrDefault(e => e.Start == currentNode);
            if (edge == null)
            {
                completed = false;
                break;
            }
            if (edges.Contains(edge))
            {
                completed = false;
                break;
            }
            edges.Add(edge);
            currentNode = nextNode;
        }

        return new(completed ? PathType.Complete : PathType.ClosestApproach, edges.ToArray());
    }
}
