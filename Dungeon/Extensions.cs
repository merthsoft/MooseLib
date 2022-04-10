using Merthsoft.Moose.Dungeon.Tiles;

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

    public static int TreasureValue(this ItemTile t)
        => t switch
        {
            var x when x < ItemTile.TREASURE_START => -1,
            var x when x >= ItemTile.TREASURE_END => -1,
            ItemTile.SmallSilverPile => 1,
            ItemTile.MediumSilverPile => 2,
            ItemTile.LargeSilverPile => 3,
            ItemTile.SmallGoldPile => 4,
            ItemTile.MediumGoldPile => 5,
            ItemTile.LargeGoldPile => 6,
            ItemTile.SilverRing => 7,
            ItemTile.BronzeRing => 8,
            ItemTile.GoldenRing => 9,
            ItemTile.SilverCrossNecklace => 10,
            ItemTile.GoldCrossNecklace => 11,
            ItemTile.EmeraldNecklace => 12,
            ItemTile.FoxNecklace => 13,
            ItemTile.TopazNecklace => 14,
            ItemTile.DiamondNecklace => 15,
            ItemTile.JadeNecklace => 16,
            _ => -1
        };

    public static Vector2 GetDelta(this string move)
        => move switch
        {
            "Left" => new(-1, 0),
            "Right" => new(1, 0),
            "Down" => new(0, 1),
            _ => new(0, -1),
        };
}
