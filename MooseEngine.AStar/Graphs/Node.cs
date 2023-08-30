namespace Merthsoft.Moose.MooseEngine.PathFinding.Graphs;

public sealed class Node : INode
{
    private Dictionary<INode, (Edge i, Edge o)> EdgeDictionary = new();

    public Node(Vector2 position)
    {
        Incoming = new List<IEdge>(0);
        Outgoing = new List<IEdge>(0);

        this.Position = position;
    }

    public Node(float x, float y)
    {
        Incoming = new List<IEdge>(0);
        Outgoing = new List<IEdge>(0);

        Position = new(x, y);
    }

    public IList<IEdge> Incoming { get; }

    public IList<IEdge> Outgoing { get; }

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

    public (Edge i, Edge o) GetEdgeTuple(INode n)
        => EdgeDictionary[n];

    public void ConnectIncoming(INode node, Velocity traversalVelocity)
    {
        if (!EdgeDictionary.TryGetValue(node, out var edge))
        {
            edge = (new Edge(this, node, traversalVelocity) { IsIncoming  = true }, new Edge(this, node, traversalVelocity));
            EdgeDictionary[node] = edge;
        }

        if (!edge.i.IsConnected)
        {
            Incoming.Add(edge.i);
            edge.i.IsConnected = true;
        }
    }

    public void ConnectOutgoing(INode node, Velocity traversalVelocity)
    {
        if (!EdgeDictionary.TryGetValue(node, out var edge))
        {
            edge = (new Edge(this, node, traversalVelocity) { IsIncoming = true }, new Edge(this, node, traversalVelocity));
            EdgeDictionary[node] = edge;
        }

        if (!edge.o.IsConnected)
        {
            Outgoing.Add(edge.o);
            edge.o.IsConnected = true;
        }
    }

    public void DisconnectAll()
    {
        foreach (var edge in Outgoing)
        {
            edge.End.Disconnect(this);
            edge.IsConnected = false;
        }
        Outgoing.Clear();
    }

    public void DisconnectOutgoing()
    {
        foreach (var edge in Outgoing)
        {
            edge.IsConnected = false;
        }
        Outgoing.Clear();
    }

    public void DisconnectIncoming()
    {
        foreach (var edge in Incoming)
        {
            edge.IsConnected = false;
        }
        Incoming.Clear();
    }

    public void Disconnect(INode node)
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
}
