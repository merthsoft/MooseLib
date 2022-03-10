namespace Merthsoft.Moose.Dungeon.Entities;
public abstract class DungeonCreature : DungeonObject
{
    public const string Left = "Left";
    public const string Right = "Right";
    public const string Up = "Up";
    public const string Down = "Down";

    public DungeonCreatureDef DungeonCreatureDef { get; }
    protected bool[,] SightMap { get; set; } = new bool[0, 0];
    public int ViewRadius { get; set; } = 8;
    public bool UseVisionCircle { get; set; } = true;

    public List<DungeonCreature> VisibleMonsters { get; } = new();

    public DungeonCreature(DungeonCreatureDef def, Vector2? position, string direction, float? rotation = null, Vector2? size = null, string layer = "")
        : base(def, position, direction, rotation, size, layer)
    {
        Layer = layer ?? def.DefaultLayer;
        DungeonCreatureDef = def;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        RebuildSightMap((game as DungeonGame)!);
        base.Update(game, gameTime);
    }

    public override void PostUpdate(MooseGame game, GameTime gameTime)
    {
        RebuildSightMap((game as DungeonGame)!);
        base.PostUpdate(game, gameTime);
    }

    protected virtual void SetTileVisible(DungeonGame dungeonGame, int x, int y)
        => SightMap[x, y] = true;

    public virtual void RebuildSightMap(DungeonGame dungeonGame)
    {
        var (dungeonWidth, dungeonHeight) = (dungeonGame.DungeonSize, dungeonGame.DungeonSize);
        SightMap = new bool[dungeonWidth, dungeonHeight];
        var visibleMonsters = new List<(float, DungeonCreature)>();
        var cell = GetCell();
        var (creatureX, creatureY) = cell;
        SetTileVisible(dungeonGame, creatureX, creatureY);

        for (var d = 0f; d < MathF.PI * 2; d += .01f)
            for (var delta = 1f; delta < ViewRadius; delta += 1)
            {
                var posX = (creatureX + delta * MathF.Cos(d)).Round();
                var posY = (creatureY + delta * MathF.Sin(d)).Round();

                if (posX < 0 || posY < 0 || posX >= dungeonWidth || posY >= dungeonHeight)
                    continue;

                SetTileVisible(dungeonGame, posX, posY);
                if (dungeonGame.GetDungeonTile(posX, posY).BlocksSight())
                    break;

                var monster = dungeonGame.GetMonster(posX, posY);
                if (monster != null && monster != this)
                {
                    var distance = DistanceSquaredTo(monster);
                    visibleMonsters.Add((distance, monster));
                }
            }

        VisibleMonsters.Clear();
        VisibleMonsters.AddRange(visibleMonsters.OrderBy(v => v.Item1).Select(v => v.Item2));
    }

    public bool CanSee(int i, int j)
        => ParentMap.CellIsInBounds(i, j) && (!UseVisionCircle || SightMap[i, j]);

    public bool CanSee(Vector2 worldPosition)
    {
        if (ParentMap.WorldIsInBounds(worldPosition))
            return !UseVisionCircle || SightMap[(int)worldPosition.X / ParentMap.TileWidth, (int)worldPosition.Y / ParentMap.TileHeight];
        else
            return false;
    }

    protected static string? DirectionFrom(Point diff) => (diff.X, diff.Y) switch
    {
        (-1, 0) => Right,
        (1, 0) => Left,
        (0, -1) => Down,
        (0, 1) => Up,
        _ => null,
    };

    protected void CalculateDirection() => (Rotation, Effects) = Direction switch
    {
        Up => (-MathF.PI / 2, SpriteEffects.None),
        Down => (MathF.PI / 2, SpriteEffects.None),
        Left => (0, SpriteEffects.FlipHorizontally),
        _ => (0, SpriteEffects.None),
    };
}
