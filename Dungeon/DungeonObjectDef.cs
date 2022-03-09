using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon;

public abstract record DungeonObjectDef : GameObjectDef
{
    public DungeonObjectDef(string defName) : base(defName)
    {
        DefaultLayer = "dungeon";
        DefaultSize = new(16, 16);
    }
}
