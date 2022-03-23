namespace Merthsoft.Moose.Dungeon.Entities;
public abstract record DungeonCreatureDef : DungeonObjectDef
{
    public DungeonCreatureDef(string defName, string layer, string? imageName = null)
        : base(defName, imageName ?? defName)
    {
        DefaultLayer = layer;
        DefaultSize = new(16, 16);
    }
}

public abstract class DungeonCreature : DungeonObject
{
    public const string Left = "Left";
    public const string Right = "Right";
    public const string Up = "Up";
    public const string Down = "Down";

    public DungeonCreatureDef DungeonCreatureDef;
    protected FogOfWar[,] SightMap = new FogOfWar[0, 0];
    public int ViewRadius = 8;
    public bool UseVisionCircle = true;
    public bool BuildSightMap = false;

    public readonly List<DungeonCreature> VisibleMonsters = new();

    public int FrozenTurnCount;

    public int SeenCount = 0;

    public DungeonCreature(DungeonCreatureDef def, Vector2? position, string direction, float? rotation = null, Vector2? size = null, string layer = "")
        : base(def, position, direction, rotation, size, layer)
    {
        Layer = layer ?? def.DefaultLayer;
        DungeonCreatureDef = def;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        if (BuildSightMap)
            RebuildSightMap((game as DungeonGame)!);
        base.Update(game, gameTime);
    }

    public override void PostUpdate(MooseGame game, GameTime gameTime)
    {
        FrozenTurnCount--;
        base.PostUpdate(game, gameTime);
    }

    public virtual void ResetVision()
    {
        var dungeonGame = DungeonGame.Instance;
        VisibleMonsters.Clear();
        SightMap = new FogOfWar[dungeonGame.DungeonSize, dungeonGame.DungeonSize];
        UseVisionCircle = true;
    }

    protected virtual void SetTileVisible(DungeonGame dungeonGame, int x, int y)
    {
        if (SightMap[x, y] == FogOfWar.Full)
            CellDiscovered(x, y);
        SightMap[x, y] = FogOfWar.None;
    }

    protected virtual void CellDiscovered(int x, int y) { }

    public virtual void RebuildSightMap(DungeonGame dungeonGame)
    {
        var (dungeonWidth, dungeonHeight) = (dungeonGame.DungeonSize, dungeonGame.DungeonSize);

        if (SightMap.Length == 0)
            ResetVision();

        for (var x = 0; x < dungeonWidth; x++)
            for (var y = 0; y < dungeonHeight; y++)
                SightMap[x, y] = 
                    SightMap[x, y] == FogOfWar.None 
                    ? FogOfWar.Half 
                    : SightMap[x, y];

        var visibleMonsters = new List<(float distance, DungeonCreature creature)>();
        var cell = GetCell();
        var (creatureX, creatureY) = cell;
        SetTileVisible(dungeonGame, creatureX, creatureY);

        for (var d = 0f; d < MathF.PI * 2; d += .05f)
            for (var delta = 1f; delta < ViewRadius; delta += 1)
            {
                var posX = (creatureX + delta * MathF.Cos(d)).Round();
                var posY = (creatureY + delta * MathF.Sin(d)).Round();

                if (posX < 0 || posY < 0 || posX >= dungeonWidth || posY >= dungeonHeight)
                    continue;

                SetTileVisible(dungeonGame, posX, posY);

                var monster = dungeonGame.GetMonster(posX, posY);
                if (monster != null && monster != this && !visibleMonsters.Any(t => t.creature == monster))
                {
                    var distance = DistanceSquaredTo(monster);
                    visibleMonsters.Add((distance, monster));
                    if (this == DungeonPlayer.Instance)
                        monster.SeenCount++;
                }
                else if (dungeonGame.GetDungeonTile(posX, posY).BlocksSight())
                    break;
            }

        VisibleMonsters.Clear();
        VisibleMonsters.AddRange(visibleMonsters.OrderBy(v => v.Item1).Select(v => v.Item2));
    }

    public FogOfWar CanSee(int i, int j)
        => !ParentMap.CellIsInBounds(i, j)
            ? FogOfWar.Full
            : UseVisionCircle
                ? SightMap[i, j]
                : FogOfWar.None;

    public FogOfWar CanSee(Vector2 worldPosition)
        => CanSee((int)(worldPosition.X / 16), (int)(worldPosition.Y / 16));

    protected static string? DirectionFrom(Point diff) => (diff.X, diff.Y) switch
    {
        (-1, 0) => Right,
        (1, 0) => Left,
        (0, -1) => Down,
        (0, 1) => Up,
        _ => null,
    };

    protected float CalculateRotation(string direction) => direction switch
    {
        Up => -MathF.PI / 2,
        Down => MathF.PI / 2,
        Left => MathF.PI,
        _ => 0,
    };

    protected virtual void ProcessMove(string move)
    {
        var moveDelta = Vector2.Zero;
        var newDirection = "";
        switch (move)
        {
            case Left:
                moveDelta = new(-1, 0);
                newDirection = Left;
                break;
            case Right:
                moveDelta = new(1, 0);
                newDirection = Right;
                break;
            case Down:
                moveDelta = new(0, 1);
                newDirection = Down;
                break;
            case Up:
                moveDelta = new(0, -1);
                newDirection = Up;
                break;
        }

        Direction = newDirection;
        Rotation = CalculateRotation(Direction);

        this.AddTween(p => p.AnimationRotation, Direction == Left ? -1 : 1, .075f,
            onEnd: _ => this.AddTween(p => p.AnimationRotation, 0, .075f));

        if (moveDelta != Vector2.Zero)
        {
            var newcell = Position / 16 + moveDelta;
            var blocked = game.IsCellBlocked((int)newcell.X, (int)newcell.Y);
            if (!blocked)
            {
                CurrentlyBlockingInput = true;
                moveDelta *= 16;
                Position += moveDelta;
                AnimationPosition = -moveDelta;
                this.AddTween(p => p.AnimationPosition, Vector2.Zero, .2f,
                    onEnd: _ =>
                    {
                        CurrentlyBlockingInput = false;
                        AnimationPosition = Vector2.Zero;
                    });
            }
            
            var playerCell = game.Player.GetCell();
            if (playerCell == newcell.ToPoint())
                player.TakeDamage(1);

        }
    }
}
