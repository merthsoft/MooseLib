﻿namespace Merthsoft.Moose.Dungeon.Tiles;

public enum MiniMapTile
{
    None = -1,

    FLOOR_START = 0,
    GrayLargeCheckersFloor = 0,
    GraySmallCheckersFloor,
    GrayHollowCheckersFloor,
    GrayBrickFloor,
    GrayCobblestoneFloor,
    GrayHollowBrickFloor,
    GrayCirclesFloor,
    GrayPlusFloor,
    GrayStoneFloor,
    GrayDotsFloor,
    RedLargeCheckersFloor,
    RedSmallCheckersFloor,
    RedHollowCheckersFloor,
    RedBrickFloor,
    RedCobblestoneFloor,
    RedHollowBrickFloor,
    RedCirclesFloor,
    RedPlusFloor,
    RedStoneFloor,
    RedDotsFloor,
    FLOOR_END,

    DOOR_START = 20,
    GreenDoorHorizontal = 20,
    GreenDoorVertical,
    RedDoorHorizontal,
    RedDoorVertical,
    DOOR_END,

    GraySpikes = 30,
    RedSpikes = 31,
    Drawers = 32,
    Shelf1,
    Shelf2,
    Shelf3,
    ClosedChest,
    OpenChest,

    StairsDown = 40,
    StairsUp,

    TableLeft = 60,
    TableMiddle,
    TableRight,
    ChairRight,
    ChairLeft,
    ChairDown,
    ChairUp,

    WALL_START = 80,
    StoneWall = 80,
    BrickWall,
    WALL_END,

    Monster = 90,
    Player,
    Chest,
    Key,
    Necklace,
    Scroll,
    Potion
}
