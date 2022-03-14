namespace Merthsoft.Moose.Dungeon.Entities.Items;

public abstract class InteractiveItem : DungeonItem
{
    public bool InteractedWith;

    public InteractiveItem(ItemDef def, Vector2 position) : base(def, position)
    {
    }

    public virtual void Interact()
    {
        InteractedWith = true;

        if (BeforeGrow())
            TweenToScale(new(2, 2), .25f,
                onEnd: _ =>
                {
                    if (AfterGrow())
                        TweenToScale(new(1, 1), .25f, delay: .25f,
                            onEnd: _ => Remove = !AfterShrink());
                });
    }

    public virtual bool BeforeGrow() => true;
    public virtual bool AfterGrow() => true;
    public virtual bool AfterShrink() => true;
}
