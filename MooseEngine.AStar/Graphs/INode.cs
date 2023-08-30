namespace Merthsoft.Moose.MooseEngine.PathFinding.Graphs;

public interface INode
{
    Vector2 Position { get; }
    Point PointPosition { get; }

    IList<IEdge> Incoming { get; }

    IList<IEdge> Outgoing { get; }

    void Disconnect(INode node);
    (Edge i, Edge o) GetEdgeTuple(INode node);
}
