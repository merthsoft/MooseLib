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

    public Chest(ChestDef def, Vector2 position) : base(def, position)
    {
        Def = def;
        DrawIndex = (int)ItemTile.ClosedChest;
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
                spiralEnumerator.MoveNext();
                cell = spiralEnumerator.Current;
            } while (DungeonGame.Instance.IsCellOccupied(cell.X, cell.Y));
            DungeonGame.Instance.SpawnItem(item, cell.X, cell.Y);
        }
    }
}