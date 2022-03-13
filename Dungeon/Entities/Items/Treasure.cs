using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public record TreasureDef : ItemDef
{
    public int Value;
    public TreasureDef(ItemTile item, string name) : base(item, name, false)
    {
        Value = item.TreasureValue();
    }
}

public class Treasure : PickableItem
{
    public TreasureDef TreasureDef;

    public Treasure(TreasureDef def, Vector2 position) : base(def, position)
    {
        TreasureDef = def;
    }

    public override void PickUp()
        => game.Player.GiveGold(TreasureDef.Value);
}
