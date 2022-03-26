using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items.Scrolls;

public record ScrollDef(ItemTile item, string name) : UsableItemDef(item, name);

public abstract class Scroll : UsableItem
{
    public Scroll(ItemTile itemTile, ScrollDef def, Vector2 position) : base(def, position)
    {
        MiniMapTile = MiniMapTile.Potion;
        DrawIndex = (int)itemTile;
    }

    protected abstract IEnumerable<Vector2> AllowedCells();

    public override void Use()
    {
        base.Use();
        player.StartBlink(AllowedCells());
    }
}
