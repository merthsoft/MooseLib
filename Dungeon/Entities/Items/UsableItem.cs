using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;
public record UsableItemDef(ItemTile item, string name) : ItemDef(item, name, false)
{
    public bool RemoveOnUse = true;
}

public class UsableItem : PickableItem
{
    public UsableItemDef UsableItemDef;
    public bool Identified = true;

    public string DisplayName => Identified ? UsableItemDef.Name : "Unknown";

    public UsableItem(UsableItemDef def, Vector2 position) : base(def, position)
    {
        UsableItemDef = def;
    }

    public override void PickUp()
        => DungeonPlayer.Instance.GiveItem(this);

    public virtual void Use()
    {
        if (UsableItemDef.RemoveOnUse)
        {
            DungeonPlayer.Instance.TakeItem(this);
            Remove = true;
        }
    }
}
