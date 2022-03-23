using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public record PotionDef(ItemTile item, string name) : UsableItemDef(item, name);

public abstract class Potion : UsableItem
{
    public Potion(ItemTile itemTile, PotionDef def, Vector2 position) : base(def, position)
    {
        MiniMapTile = Tiles.MiniMapTile.Potion;
        DrawIndex = (int)itemTile;
    }

    public override void Use()
    {
        base.Use();
        Drink();
    }

    public abstract void Drink();
}
