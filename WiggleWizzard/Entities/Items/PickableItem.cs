namespace Merthsoft.Moose.Dungeon.Entities.Items;

public abstract class PickableItem : InteractiveItem
{
    public PickableItem(ItemDef def, int x, int y) : base(def, x, y)
    {
        State = "inanimate";
    }

    public override void Interact()
    {
        if (InteractedWith)
            return;

        LayerDepth = 1;
        PickUp();
        base.Interact();
    }

    public abstract void PickUp();

    public override bool AfterGrow()
    {
        LayerDepth = .5f;
        Remove = true;
        return false;
    }
}
