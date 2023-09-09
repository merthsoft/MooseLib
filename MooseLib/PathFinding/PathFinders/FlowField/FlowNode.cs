namespace Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;

public record FlowNode(int NextX, int NextY, float CostValue)
{
    public bool Valid => NextX != -1 && NextY != -1;
}
