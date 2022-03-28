using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public record ChestDef : ItemDef
{
    public ChestDef() : base(ItemTile.ClosedChest, "Chest", true)
    {
        MiniMapTile = MiniMapTile.Chest;
    }

    public override void LoadContent(MooseContentManager contentManager) => base.LoadContent(contentManager);
}

public class Chest : InteractiveItem
{
    public bool IsOpen = false;
    public List<ItemTile> Contents = new();
    public bool IsLocked = false;

    public Chest(ChestDef def, Vector2 position) : base(def, position)
    {
        Def = def;
        DrawIndex = (int)ItemTile.ClosedChest;
    }

    public override void Interact()
    {
        if (IsLocked)
        {
            game.SpawnFallingText("Locked", Position, Color.DarkGray);
            CurrentlyBlockingInput = true;
            this.AddTween(p => p.AnimationRotation, -1, .075f,
                onEnd: _ => this.AddTween(p => p.AnimationRotation, 1, .075f,
                    onEnd: _ =>
                    {
                        AnimationRotation = 0;
                        CurrentlyBlockingInput = false;
                    }));
            return;
        }
        base.Interact();
    }

    public override bool AfterGrow()
    {
        IsOpen = true;
        DrawIndex = (int)ItemTile.OpenChest;
        SpawnItems();
        return true;
    }

    public void SpawnItems()
    {
        var spiralEnumerator = GetCell().SpiralAround().GetEnumerator();
        foreach (var item in Contents)
        {
            Point cell;
            do
            {
                cell = spiralEnumerator.MoveNextGetCurrent();
            } while (DungeonGame.Instance.IsCellOccupied(cell.X, cell.Y));
            DungeonGame.Instance.SpawnItem(item, cell.X, cell.Y);
        }
    }
}