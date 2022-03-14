namespace Merthsoft.Moose.Dungeon.Entities.Items;

public abstract class PickableItem : InteractiveItem
{
    public PickableItem(ItemDef def, Vector2 position) : base(def, position)
    {
        State = "inanimate";
    }

    public override void Interact()
    {
        if (InteractedWith)
            return;

        PickUp();
        base.Interact();
    }

    public abstract void PickUp();

    public override bool AfterGrow()
        => !(Remove = true);
}
