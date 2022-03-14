using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon.Entities;
internal record DungeonRoomDef(float Weight) : Def("Room")
{
    public int NumTreasure { get; init; }
    public int NumChests { get; init; }
    public int NumPotions { get; init; }
    public int NumScrolls { get; init; }
    public int NumMonsters { get; init; }
    public int MonsterLevelDelta { get; init; }

    private int roomNumber;
    public int RoomNumber { 
        get => roomNumber; 
        set {
            roomNumber = value;
            DefName = $"Room {roomNumber}";
        }
    }
}
