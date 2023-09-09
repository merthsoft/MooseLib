namespace Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
public struct AdjacentTile<T>
{
    public int XOffset;
    public int YOffset;
    public T Value;

    internal void Deconstruct(out int xOffset, out int yOffset, out T value)
        => (xOffset, yOffset, value)
         = (XOffset, YOffset, Value);
}
