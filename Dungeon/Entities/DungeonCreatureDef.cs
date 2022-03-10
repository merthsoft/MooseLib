using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon.Entities;

public abstract record DungeonCreatureDef : DungeonObjectDef
{
    public DungeonCreatureDef(string defName, string layer, string? imageName = null) 
        : base(defName, imageName ?? defName)
    {
        DefaultLayer = layer;
        DefaultSize = new(16, 16);
    }
}
