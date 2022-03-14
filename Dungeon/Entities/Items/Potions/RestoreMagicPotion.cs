using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public class RestoreMagicPotion : Potion
{
    public RestoreMagicPotion(ItemTile potionTile, UsableItemDef def, Vector2 position) : base(potionTile, def, position)
    {
    }

    public override void Drink()
        => player.HealMp(10);
}
