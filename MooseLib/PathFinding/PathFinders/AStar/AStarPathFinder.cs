namespace Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.AStar;
public sealed class AStarPathFinder : IPathFinder
{
    private readonly MinHeap<AStarPathFinderNode> Interesting;
    private readonly Dictionary<Node, AStarPathFinderNode> Nodes;
    private readonly PathReconstructor PathReconstructor;

    private AStarPathFinderNode? NodeClosestToGoal;

    public uint MaxDepth { get; set; }

    public AStarPathFinder()
    {
        Interesting = new MinHeap<AStarPathFinderNode>();
        Nodes = new Dictionary<Node, AStarPathFinderNode>();
        PathReconstructor = new PathReconstructor();
    }

    public Path FindPath(int x1, int y1, int x2, int y2, Grid grid, Velocity? maxVel = null)
    {
        ResetState();

        var start = grid.GetNode(x1, y1);
        var goal = grid.GetNode(x2, y2);

        var maximumVelocity = maxVel ?? grid.GetAllNodes().SelectMany(n => n.Outgoing).Select(e => e.TraversalVelocity).Max();

        AddFirstNode(start, goal, maximumVelocity);

        var depth = 0;
        while (Interesting.Count > 0 && (MaxDepth == 0 || depth < MaxDepth))
        {
            depth++;
            var current = Interesting.Extract();
            if (GoalReached(goal, current))
                return PathReconstructor.ConstructPathTo(current.Node, goal);

            UpdateNodeClosestToGoal(current);

            foreach (var edge in current.Node.Outgoing)
            {
                if (!edge.IsConnected)
                    continue;

                var oppositeNode = edge.End;

                var costSoFar = current.DurationSoFar + edge.TraversalDuration;

                if (Nodes.TryGetValue(oppositeNode, out var node))
                    UpdateExistingNode(goal, maximumVelocity, current, edge, oppositeNode, costSoFar, node);
                else
                    InsertNode(oppositeNode, edge, goal, costSoFar, maximumVelocity);
            }
        }

        return PathReconstructor.ConstructPathTo(NodeClosestToGoal!.Node, goal);
    }

    private void ResetState()
    {
        Interesting.Clear();
        Nodes.Clear();
        PathReconstructor.Clear();
        NodeClosestToGoal = null;
    }

    private void AddFirstNode(Node start, Node goal, Velocity maximumVelocity)
    {
        var head = new AStarPathFinderNode(start, Duration.Zero, ExpectedDuration(start, goal, maximumVelocity));
        Interesting.Insert(head);
        Nodes.Add(head.Node, head);
        NodeClosestToGoal = head;
    }

    private static bool GoalReached(Node goal, AStarPathFinderNode current) => current.Node == goal;

    private void UpdateNodeClosestToGoal(AStarPathFinderNode current)
    {
        if (current.ExpectedRemainingTime < NodeClosestToGoal!.ExpectedRemainingTime)
            NodeClosestToGoal = current;
    }

    private void UpdateExistingNode(Node goal, Velocity maximumVelocity, AStarPathFinderNode current, Edge edge, Node oppositeNode, Duration costSoFar, AStarPathFinderNode node)
    {
        if (node.DurationSoFar > costSoFar)
        {
            Interesting.Remove(node);
            InsertNode(oppositeNode, edge, goal, costSoFar, maximumVelocity);
        }
    }

    private void InsertNode(Node current, Edge via, Node goal, Duration costSoFar, Velocity maximumVelocity)
    {
        PathReconstructor.SetCameFrom(current, via);

        var node = new AStarPathFinderNode(current, costSoFar, ExpectedDuration(current, goal, maximumVelocity));
        Interesting.Insert(node);
        Nodes[current] = node;
    }

    public static Duration ExpectedDuration(Node a, Node b, Velocity maximumVelocity)
        => Distance.BeweenPositions(a.Position, b.Position) / maximumVelocity;
}
