namespace Merthsoft.Moose.MooseEngine.PathFinding.Serialization;

public class EdgeDto
{
    public Merthsoft.Moose.MooseEngine.PathFinding.Serialization.VelocityDto TraversalVelocity { get; set; }

    public Merthsoft.Moose.MooseEngine.PathFinding.Serialization.GridPositionDto Start { get; set; }

    public Merthsoft.Moose.MooseEngine.PathFinding.Serialization.GridPositionDto End { get; set; }
}
