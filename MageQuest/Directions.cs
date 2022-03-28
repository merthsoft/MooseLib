namespace Merthsoft.Moose.MageQuest;

public static class Directions
{
    public const string South = "south";
    public const string North = "north";
    public const string East = "east";
    public const string West = "west";

    public static Vector2 SouthVector = new(0, 1);
    public static Vector2 NorthVector = new(0, -1);
    public static Vector2 EastVector = new(1, 0);
    public static Vector2 WestVector = new(-1, 0);

    public static Vector2 GetVector(string direction)
        => direction switch
        {
            South => SouthVector,
            North => NorthVector,
            East => EastVector,
            West => WestVector,
            _ => Vector2.Zero,
        };
}
