using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities;

public record ItemDef : DungeonObjectDef
{
    public string Name;
    public ItemTile Item;

    public ItemDef(ItemTile item, string name) : base(item.ToString(), "Items")
    {
        Name = name;
        Item = item;
    }
}

public abstract class Item : DungeonObject
{
    public ItemDef ItemDef;
    public override int DrawIndex => (int)ItemDef.Item;

    private bool PickedUp = false;

    public Item(ItemDef def, Vector2 position) : base(def, position, "", 0, new Vector2(16, 16), "items")
    {
        ItemDef = def;
        State = "inanimate";
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
        if (DungeonPlayer.Instance.GetCell() == GetCell() && !PickedUp)
        {
            CurrentlyBlockingInput = true;
            PickedUp = true;
            PickUp();
            TweenToScale(new(2, 2), 1, onEnd: _ => Remove = true);
        }
    }

    public abstract void PickUp();
}
