using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public record ChestDef : ItemDef
{
    public ChestDef() : base(ItemTile.ClosedChest, "Chest", true)
    {

    }
}

public class Chest : InteractiveItem
{
    public override int DrawIndex => IsOpen ? base.DrawIndex + 1 : base.DrawIndex;

    public bool IsOpen = false;
    public List<DungeonObjectDef> Contents = new();

    public Chest(Vector2 position) : base((ItemDef)MooseEngine.Defs.Def.Empty, position)
    {
        Def = DungeonGame.GetDef<ChestDef>("Chest");
    }

    
}
