
namespace Merthsoft.Moose.MooseEngine.PathFinding.Graphs;

public interface IEdge
{
    Velocity TraversalVelocity { get; set; }

    Duration TraversalDuration { get; }

    Distance Distance { get; }

    INode Start { get; }

    INode End { get; }
    bool IsConnected { get; set; }
}