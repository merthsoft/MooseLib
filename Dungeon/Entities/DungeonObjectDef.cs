using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon.Entities;

public abstract record DungeonObjectDef : TextureGameObjectDef
{
    public int HitPoints { get; set; }

    public DungeonObjectDef(string DefName, string TextureName, Rectangle? SourceRectangle = null) : base(DefName, TextureName, SourceRectangle)
    {
        DefaultLayer = "dungeon";
        DefaultSize = new(16, 16);
    }
}
