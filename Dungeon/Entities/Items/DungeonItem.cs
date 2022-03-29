using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public record ItemDef : DungeonObjectDef
{
    public string Name;
    public ItemTile Item;
    public bool BlocksPlayer;

    public ItemDef(ItemTile item, string name, bool blocksPlayer) : base(item.ToString(), "Items")
    {
        Name = name;
        Item = item;
        BlocksPlayer = blocksPlayer;
        DrawIndex = (int)item;
    }
}

public abstract class DungeonItem : DungeonObject
{
    public ItemDef ItemDef;

    public DungeonItem(ItemDef def, int x, int y) : base(def, new(x * 16, y * 16), "", 0, new Vector2(16, 16), "items")
    {
        ItemDef = def;
        State = "inanimate";
        DrawIndex = (int)ItemDef.Item;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
    }
}
