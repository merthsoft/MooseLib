namespace Merthsoft.Moose.Dungeon;
public static class Extensions
{
    public static bool IsWall(this DungeonTile t)
        => t >= DungeonTile.WALL_START && t < DungeonTile.WALL_END;

    public static bool IsFloor(this DungeonTile t)
        => t >= DungeonTile.FLOOR_START && t < DungeonTile.FLOOR_END;

    public static bool IsDoor(this DungeonTile t)
        => t >= DungeonTile.DOOR_START && t < DungeonTile.DOOR_END;

    public static bool IsBlocking(this DungeonTile t)
        => t.IsWall();

    public static bool BlocksSight(this DungeonTile t)
        => t.IsWall() || t.IsDoor();
}
