
using Merthsoft.Moose.MooseEngine.PathFinding.Collections;

namespace Merthsoft.Moose.MooseEngine.PathFinding.Paths.AStar; 
public sealed class AStarPathFinder : IPathFinder
{
    private readonly MinHeap<AStarPathFinderNode> Interesting;
    private readonly Dictionary<INode, AStarPathFinderNode> Nodes;
    private readonly PathReconstructor PathReconstructor;

    private AStarPathFinderNode? NodeClosestToGoal;

    public uint MaxDepth { get; set; }

    public AStarPathFinder()
    {
        Interesting = new MinHeap<AStarPathFinderNode>();
        Nodes = new Dictionary<INode, AStarPathFinderNode>();
        PathReconstructor = new PathReconstructor();
    }

    public Path FindPath(INode start, INode goal, Velocity maximumVelocity)
    {
        ResetState();
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

                if (!oppositeNode.GetEdgeTuple(edge.Start).i.IsConnected)
                    continue;

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

    private void AddFirstNode(INode start, INode goal, Velocity maximumVelocity)
    {
        var head = new AStarPathFinderNode(start, Duration.Zero, ExpectedDuration(start, goal, maximumVelocity));
        Interesting.Insert(head);
        Nodes.Add(head.Node, head);
        NodeClosestToGoal = head;
    }

    private static bool GoalReached(INode goal, AStarPathFinderNode current) => current.Node == goal;

    private void UpdateNodeClosestToGoal(AStarPathFinderNode current)
    {
        if (current.ExpectedRemainingTime < NodeClosestToGoal!.ExpectedRemainingTime)
            NodeClosestToGoal = current;
    }

    private void UpdateExistingNode(INode goal, Velocity maximumVelocity, AStarPathFinderNode current, IEdge edge, INode oppositeNode, Duration costSoFar, AStarPathFinderNode node)
    {
        if (node.DurationSoFar > costSoFar)
        {
            Interesting.Remove(node);
            InsertNode(oppositeNode, edge, goal, costSoFar, maximumVelocity);
        }
    }

    private void InsertNode(INode current, IEdge via, INode goal, Duration costSoFar, Velocity maximumVelocity)
    {
        PathReconstructor.SetCameFrom(current, via);

        var node = new AStarPathFinderNode(current, costSoFar, ExpectedDuration(current, goal, maximumVelocity));
        Interesting.Insert(node);
        Nodes[current] = node;
    }

    public static Duration ExpectedDuration(INode a, INode b, Velocity maximumVelocity)
        => Distance.BeweenPositions(a.Position, b.Position) / maximumVelocity;
}
