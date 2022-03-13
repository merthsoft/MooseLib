using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities;

public record ItemDef : DungeonObjectDef
{
    public string Name;
    public ItemTile Item;
    public bool BlocksPlayer;

    public ItemDef(ItemTile item, string name, bool blocksPlayer) : base(item.ToString(), "Items")
    {
        Name = name;
        Item = item;
    }
}

public abstract class Item : DungeonObject
{
    public ItemDef ItemDef;
    public override int DrawIndex => (int)ItemDef.Item;

    public Item(ItemDef def, Vector2 position) : base(def, position, "", 0, new Vector2(16, 16), "items")
    {
        ItemDef = def;
        State = "inanimate";
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
    }
}
