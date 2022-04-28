namespace RayMapEditor;
public record SaveMap
{
    public string Name { get; set; } = "New Map";
    public int Width { get; set; }
    public int Height { get; set; }

    public int[,] Walls { get; set; }
    public int[,] Objects { get; set; }
}
