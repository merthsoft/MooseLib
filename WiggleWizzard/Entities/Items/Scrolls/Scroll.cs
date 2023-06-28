using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items.Scrolls;

public record ScrollDef(ItemTile item, string name) : UsableItemDef(item, name);

public abstract class Scroll : UsableItem
{
    public Scroll(ScrollDef def, int x, int y) : base(def, x, y)
    {
        MiniMapTile = MiniMapTile.Potion;
        DrawIndex = (int)def.item;
    }

    protected abstract IEnumerable<Point> AllowedCells();

    public override void Use()
    {
        base.Use();
        player.StartBlink(AllowedCells());
    }
}
