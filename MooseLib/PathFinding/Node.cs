namespace Merthsoft.Moose.MooseEngine.PathFinding;

public class Node
{
    private readonly Dictionary<Node, Edge> OutgoingEdges = new();
    private readonly Dictionary<Node, Edge> IncomingEdges = new();

    public Node(Vector2 position)
        => Position = position;

    public Node(float x, float y)
        => Position = new(x, y);

    public IList<Edge> Incoming => IncomingEdges.Select(x => x.Value).Where(x => x.IsConnected).ToList();
    public IList<Edge> AllIncoming => IncomingEdges.Select(x => x.Value).ToList();

    public IList<Edge> Outgoing => OutgoingEdges.Select(x => x.Value).Where(x => x.IsConnected).ToList();
    public IList<Edge> AllOutgoing => OutgoingEdges.Select(x => x.Value).ToList();

    private Vector2 position;
    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
            PointPosition = value.ToPoint();
        }
    }

    public Point PointPosition { get; private set; }

    public void AddOutgoingEdge(Edge edge)
        => OutgoingEdges[edge.End] = edge;

    public void AddIncomingEdge(Edge edge)
        => IncomingEdges[edge.Start] = edge;

    public bool IsIncomingConnected(Node node)
        => IncomingEdges.TryGetValue(node, out var edge) ? edge.IsConnected : false;

    public bool IsOutgoingConnected(Node node)
        => OutgoingEdges.TryGetValue(node, out var edge) ? edge.IsConnected : false;

    public void ConnectIncoming(Node node, Velocity traversalVelocity)
    {
        if (!IncomingEdges.TryGetValue(node, out var edge))
        {
            edge = new Edge(node, this, traversalVelocity);
            node.AddOutgoingEdge(edge);
            IncomingEdges[node] = edge;
        }
        edge.IsConnected = true;
    }

    public void ConnectOutgoing(Node node, Velocity traversalVelocity)
    {
        if (!OutgoingEdges.TryGetValue(node, out var edge))
        {
            edge = new Edge(this, node, traversalVelocity);
            node.AddIncomingEdge(edge);
            OutgoingEdges[node] = edge;
        }
        edge.IsConnected = true;
    }

    public void DisconnectAll()
    {
        foreach (var edge in Outgoing)
        {
            edge.End.Disconnect(this);
            edge.IsConnected = false;
        }
    }

    public void DisconnectOutgoing()
    {
        foreach (var edge in Outgoing)
        {
            edge.IsConnected = false;
        }
    }

    public void DisconnectIncoming()
    {
        foreach (var edge in Incoming)
        {
            edge.IsConnected = false;
        }
    }

    public void Disconnect(Node node)
    {
        for (var i = Outgoing.Count - 1; i >= 0; i--)
        {
            var edge = Outgoing[i];
            if (edge.End == node)
            {
                Outgoing.Remove(edge);
                node.Incoming.Remove(edge);
                edge.Start.Incoming.Remove(edge);
                edge.IsConnected = false;
            }
        }
    }

    public override string ToString() => Position.ToString();

    public Edge GetOutgoingEdge(Node node)
        => OutgoingEdges[node];
}
