using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Monsters;
public record MonsterDef : DungeonCreatureDef
{
    public MonsterTile Monster { get; set; }

    public MonsterDef(string DefName, MonsterTile monster) : base(DefName, "monsters", "Monsters")
    {
        Monster = monster;
    }
}
