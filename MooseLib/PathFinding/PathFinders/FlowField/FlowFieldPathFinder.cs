namespace Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;

public class FlowField
{
    public FlowNode[,] CostMap { get; private set; }
    public float MaxCost { get; private set; }

    private FlowField(FlowNode[,] costMap, float maxCost)
    {
        CostMap = costMap;
        MaxCost = maxCost;
    }

    public static FlowField GenerateFlow(Grid grid, Point destination)
    {
        var (w, h) = (grid.GridSize.Width, grid.GridSize.Height);
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
                costMap[x, y] = costMap[px, py] + 1/previous.TraversalVelocity.MetersPerSecond + (node.AllOutgoingCount - node.OutgoingCount);
                if (!node.Incoming.Any())
                    costMap[x, y] += 10000;
            }
            foreach (var i in node.Incoming)
            {
                var (ix, iy) = i.Start.PointPosition;
                if (!visitedMap[ix, iy])
                    nodes.Add((i.Start, i));
            }
        }

        var convoluted = new FlowNode[w, h];
        float maxCost = 0;
        for (var x = 0; x < w; x++)
            for (var y = 0; y < h; y++)
            {
                if (costMap[x, y] > maxCost)
                    maxCost = costMap[x, y];

                if (x == destination.X && y == destination.Y)
                {
                    convoluted[x, y] = new(-1, -1, 0);
                    continue;
                }

                var node = grid.GetNode(x, y);

                //if (!node.Incoming.Any())
                //{
                //    convoluted[x, y] = new(-1, -1, costMap[x, y]);
                //    continue;
                //}
                
                var bestNode = node.Outgoing
                                    .Select(o => o.End)
                                    .GroupBy(i => costMap[i.PointPosition.X, i.PointPosition.Y])
                                    .OrderBy(g => g.Key)
                                    .FirstOrDefault()?
                                    .OrderBy(i => i.PointPosition.ManhattanDistanceTo(node.PointPosition))
                                    //.Shuffle()
                                    .FirstOrDefault();
                if (bestNode != null)
                    convoluted[x, y] = new(bestNode.PointPosition.X, bestNode.PointPosition.Y, costMap[x, y]);
                else
                    convoluted[x, y] = new(-1, -1, 10000);
            }

        return new(convoluted, maxCost - 10000);
    }
}

public class FlowFieldPathFinder : IPathFinder
{
    Dictionary<Point, FlowField> flowCache = [];

    public FlowFieldPathFinder()
    {

    }

    public void ClearCache()
        => flowCache.Clear();

    public FlowField GetFlow(Grid grid, Point endPoint)
    {
        if (flowCache.TryGetValue(endPoint, out var flowField))
            return flowField;
        return flowCache[endPoint] = FlowField.GenerateFlow(grid, endPoint);
    }

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
            var f = flow.CostMap[x, y];
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
