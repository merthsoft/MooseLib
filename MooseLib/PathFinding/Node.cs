namespace Merthsoft.Moose.MooseEngine.PathFinding;

public class Node
{
    private readonly HashSet<Edge> OutgoingEdges = [];
    private readonly HashSet<Edge> IncomingEdges = [];

    public Node(Vector2 position)
        => Position = position;

    public Node(float x, float y)
        => Position = new(x, y);

    public IEnumerable<Edge> Incoming => IncomingEdges.Where(x => x.IsConnected).ToList();
    public IEnumerable<Edge> AllIncoming => IncomingEdges.ToList();

    public int IncomingCount => IncomingEdges.Count(a => a.IsConnected);
    public int AllIncomingCount => IncomingEdges.Count;

    public IEnumerable<Edge> Outgoing => OutgoingEdges.Where(x => x.IsConnected).ToList();
    public IEnumerable<Edge> AllOutgoing => OutgoingEdges.ToList();

    public int OutgoingCount => OutgoingEdges.Count(a => a.IsConnected);
    public int AllOutgoingCount => OutgoingEdges.Count;


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
        => OutgoingEdges.Add(edge);

    public void AddIncomingEdge(Edge edge)
        => IncomingEdges.Add(edge);

    public bool IsIncomingConnected(Node node)
        => IncomingEdges.FirstOrDefault(e => e.End == node)?.IsConnected ?? false;

    public bool IsOutgoingConnected(Node node)
        => OutgoingEdges.FirstOrDefault(e => e.Start == node)?.IsConnected ?? false;

    public void ConnectIncoming(Node node, Velocity traversalVelocity)
    {
        var edge = IncomingEdges.FirstOrDefault(e => e.End == node);
        if (edge == null)
        {
            edge = new Edge(node, this, traversalVelocity);
            node.AddOutgoingEdge(edge);
            IncomingEdges.Add(edge);
        }
        edge.IsConnected = true;
    }

    public void ConnectOutgoing(Node node, Velocity traversalVelocity)
    {
        var edge = OutgoingEdges.FirstOrDefault(e => e.Start == node);
        if (edge == null)
        {
            edge = new Edge(this, node, traversalVelocity);
            node.AddIncomingEdge(edge);
            OutgoingEdges.Add(edge);
        }
        edge.IsConnected = true;
    }

    public void DisconnectAll()
    {
        foreach (var edge in OutgoingEdges)
        {
            edge.End.RemoveEdge(this);
            edge.IsConnected = false;
        }
    }

    public void DisconnectOutgoing()
    {
        foreach (var edge in OutgoingEdges)
        {
            edge.IsConnected = false;
        }
    }

    public void DisconnectIncoming()
    {
        foreach (var edge in IncomingEdges)
        {
            edge.IsConnected = false;
        }
    }

    public void ReconnectIncoming()
    {
        foreach (var edge in IncomingEdges)
        {
            edge.IsConnected = true;
        }
    }

    public void RemoveEdge(Node node)
    {
        return;
        //for (var i = OutgoingEdges.Count - 1; i >= 0; i--)
        //{
        //    var edge = Outgoing[i];
        //    if (edge.End == node)
        //    {
        //        Outgoing.Remove(edge);
        //        node.IncomingEdges.Remove(edge);
        //        edge.Start.IncomingEdges.Remove(edge);
        //        edge.IsConnected = false;
        //    }
        //}
    }

    public override string ToString() => Position.ToString();
}
