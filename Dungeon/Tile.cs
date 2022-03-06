namespace Merthsoft.Moose.Dungeon;
internal enum Tile
{
    None = -1,

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

    GreenDoorHorizontal = 20,
    GreenDoorVertical,
    RedDoorHorizontal,
    RedDoorVertical,

    BLOCKING_START = 30,
    
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
    BLOCKING_END = ChairUp,
    
    WALL_START = 80,
    StoneWall = 80,
    BrickWall,
}
