using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items.Potions;

public class RestoreMagicPotion : Potion
{
    public RestoreMagicPotion(PotionDef def, int x, int y) : base(def, x, y)
    {
    }

    public override void Drink()
        => player.HealMp(10);
}
