using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items.Potions;

public class RestoreMagicPotion : Potion
{
    public RestoreMagicPotion(PotionDef def, Vector2 position) : base(def, position)
    {
    }

    public override void Drink()
        => player.HealMp(10);
}
