using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items.Potions;

public record PotionDef(ItemTile item, string name) : UsableItemDef(item, name);

public abstract class Potion : UsableItem
{
    public Potion(PotionDef def, int x, int y) : base(def, x, y)
    {
        MiniMapTile = MiniMapTile.Potion;
        DrawIndex = (int)def.item;
    }

    public override void Use()
    {
        base.Use();
        Drink();
    }

    public abstract void Drink();
}
