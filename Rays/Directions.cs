namespace Merthsoft.Moose.Rays;
public static class Directions
{
    public static Vector2 North = new(0, 1);
    public static Vector2 NorthEast = new(.71f, .71f);
    public static Vector2 East = new(1, 0);
    public static Vector2 SouthEast = new(.71f, -.71f);
    public static Vector2 South = new(0, -1);
    public static Vector2 SouthWest = new(-.71f, -.71f);
    public static Vector2 West = new(-1, 0);
    public static Vector2 NorthWest = new(-.71f, .71f);

    public static Vector2[] CardinalDirections4 = new[] { East, North, West, South };
    public static Vector2[] CardinalDirections8 = new[] { East, NorthEast, North, NorthWest, West, SouthWest, South, SouthEast };

    public static string[] CardinalDirections4Names = new[] { "East", "North", "West", "South" };
    public static string[] CardinalDirections8Names = new[] { "East", "NorthEast", "North", "NorthWest", "West", "SouthWest", "South", "SouthEast" };
}
