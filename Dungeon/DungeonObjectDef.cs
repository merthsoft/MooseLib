using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon;

public abstract record DungeonObjectDef : GameObjectDef
{
    public DungeonObjectDef(string defName) : base(defName)
    {
        DefaultLayer = 1;
        DefaultSize = new(16, 16);
    }
}
