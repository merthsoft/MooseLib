using Merthsoft.Moose.Dungeon.Tiles;
using MonoGame.Extended.TextureAtlases;

namespace Merthsoft.Moose.Dungeon.Entities.Items;

public record ChestDef : ItemDef
{
    public ChestDef() : base(ItemTile.ClosedChest, "Chest", true)
    {
        MiniMapTile = MiniMapTile.Chest;
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        SpriteSheet = new();
        SpriteSheet.TextureAtlas = TextureAtlas.Create(AnimationKey, contentManager.LoadImage(AnimationKey), 16, 16);


        SpriteSheet.Cycles["closed"] = new()
        {
            FrameDuration = .1f,
            IsLooping = true,
            Frames = new() { new((int)ItemTile.ClosedChest), }
        }; 
        SpriteSheet.Cycles["open"] = new()
        {
            FrameDuration = .1f,
            IsLooping = true,
            Frames = new() { new((int)ItemTile.OpenChest), }
        };
    }
}

public class Chest : InteractiveItem
{
    public bool IsOpen = false;
    public List<ItemTile> Contents = new();
    public bool IsLocked = false;

    public override string PlayKey => IsOpen ? "open" : "closed";

    public Chest(ChestDef def, int x, int y) : base(def, x, y)
    {
        Def = def;
        DrawIndex = (int)ItemTile.ClosedChest;
        State = "closed";
        LayerDepth = .9f;
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
        State = "open";
        SpawnItems();
        return true;
    }

    public void SpawnItems()
    {
        var spiralEnumerator = Cell.SpiralAround().GetEnumerator();
        foreach (var item in Contents)
        {
            Point cell;
            do
            {
                cell = spiralEnumerator.MoveNextGetCurrent();
            } while (WiggleWizzardGame.Instance.IsCellOccupied(cell.X, cell.Y));
            WiggleWizzardGame.Instance.SpawnItem(item, cell.X, cell.Y);
        }
    }
}