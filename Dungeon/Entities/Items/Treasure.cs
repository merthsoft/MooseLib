using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public record TreasureDef : ItemDef
{
    public int Value;
    public TreasureDef(ItemTile item, string name) : base(item, name, false)
    {
        Value = item.TreasureValue();
        MiniMapTile = MiniMapTile.Necklace;
    }
}

public class Treasure : PickableItem
{
    public TreasureDef TreasureDef;

    public Treasure(TreasureDef def, int x, int y) : base(def, x, y)
    {
        TreasureDef = def;
    }

    public override void PickUp()
        => game.Player.GiveGold(Position, TreasureDef.Value);
}
