namespace Merthsoft.Moose.Miner
{
    enum Tile
    {
        Empty,
        Dirt1,
        Dirt2,
        Dirt3,
        
        LastDirt = Dirt3,
        
        Stone,
        Silver,
        Gold,
        Platinum,
        Clover,
        Diamond,
        Pump,
        Bucket,
        Torch,
        Shovel,
        PickAxe,
        HandDrill,
        Lantern,
        Dynamite,

        LastTreasure = Dynamite,

        SandStone = 22,
        VolcanicRock = 23,

        LastMinable = VolcanicRock,

        Sky = 24,
        Bird,
        CloudLeft,
        CloudMiddle,
        CloudRight,
        RoofLeft,
        RoofMiddle,
        RoofRight,
        Cactus,
        Hitch,
        Outhouse,
        Wall,
        Door,
        Wheel,
        HospitalSign,
        BankSign,
        ElevatorShaft,
        Elevator,
        ElevatorBottom,
        ElevatorTop_Shaft,
        ElevatorTop_Sky,
        SaloonDoor,
        SaloonSign,

        Border = 48,

        Granite = 51,
        CaveIn,
        Flood,
        Water,
        MineCeiling
    }
}
