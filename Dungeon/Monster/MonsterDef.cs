using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon.Monster;
public record MonsterDef : TextureGameObjectDef
{
    public MonsterDef(string DefName, string TextureName, int monsterIndex) : base(DefName, TextureName, null)
    {
    }
}
