namespace Merthsoft.Moose.Dungeon;
internal static class Extensions
{
    public static bool IsWall(this Tile t)
        => t >= Tile.WALL_START && t < Tile.WALL_END;

    public static bool IsFloor(this Tile t)
        => t >= Tile.FLOOR_START && t < Tile.FLOOR_END;

    public static bool IsDoor(this Tile t)
        => t >= Tile.DOOR_START && t < Tile.DOOR_END;
}
