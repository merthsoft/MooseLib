namespace Merthsoft.Moose.MooseEngine.PathFinding.Serialization;

public class NodeDto
{
    public Merthsoft.Moose.MooseEngine.PathFinding.Serialization.PositionDto Position { get; set; }

    public GridPositionDto GridPoint { get; set; }

    public List<EdgeDto> IncomingEdges { get; set; }

    public List<EdgeDto> OutGoingEdges { get; set; }
}
