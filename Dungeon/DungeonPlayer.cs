using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;

class DungeonPlayer : GameObjectBase
{
    public const string Left = "Left";
    public const string Right = "Right";
    public const string Up = "Up";
    public const string Down = "Down";

    public bool CanMove { get; set; } = true;
    
    private readonly Stack<Keys> moveBuffer = new();

    public float AnimationRotation { get; set; }

    private DungeonPlayerDef PlayerDef => (Def as DungeonPlayerDef)!;

    private bool[,] SightMap { get; set; } = new bool[0,0];

    public bool IsUnderground { get; set; } = true;
    public int ViewRadius { get; set; } = 12;

    public DungeonPlayer(DungeonPlayerDef def) : base(def)
    {

    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
    {
        var dungeonGame = (game as DungeonGame)!;

        spriteBatch.Draw(PlayerDef.Texture, WorldRectangle.Move(new(8, 8)).ToRectangle(), new(32, 48, 16, 16), Color.White, Rotation + AnimationRotation, new(8, 8), Effects, 1f);
    }

    public void RebuildSightMap(int dungeonWidth, int dungeonHeight)
    {
        SightMap = new bool[dungeonWidth, dungeonHeight];
        var cell = GetCell();
        var (playerX, playerY) = cell;
        SightMap[playerX, playerY] = true;

        for (var d = 0f; d < 360; d+=.05f)
            for (var delta = 1f; delta < ViewRadius; delta+=1)
            {
                var posX = (playerX + delta * MathF.Cos(d)).Round();
                var posY = (playerY + delta * MathF.Sin(d)).Round();

                if (posX < 0 || posY < 0 || posX >= dungeonWidth || posY >= dungeonHeight)
                    continue;

                SightMap[posX, posY] = true;
                if (ParentMap.GetBlockingVector(posX, posY)[0] != 0)
                    break;
            }
    }

    void setSightMap(IEnumerable<Point> playerSight, int dungeonWidth, int dungeonHeight)
    {
        foreach (var (posX, posY) in playerSight)
        {
            if (posX < 0 || posY < 0 || posX >= dungeonWidth || posY >= dungeonHeight)
                continue;

            SightMap[posX, posY] = true;
            if (ParentMap.GetBlockingVector(posX, posY)[0] != 0)
                break;
        }
    }

    void setSightMap(IEnumerable<Vector2> playerSight, int dungeonWidth, int dungeonHeight)
    {
        var cell = GetCell();
        foreach (var position in playerSight)
        {
            var posX = (int)(position.X / 16f);
            var posY = (int)(position.Y / 16f);
            if (posX < 0 || posY < 0 || posX >= dungeonWidth || posY >= dungeonHeight)
                continue;

            if (SightMap[posX, posY])
                continue;

            if (posX == cell.X && posY == cell.Y)
                continue;

            if (ParentMap.GetBlockingVector(posX, posY)[0] != 0)
                break;
            else
                SightMap[posX, posY] = true;
        }
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        var dungeonGame = (game as DungeonGame)!;
        bool keyPress(Keys key) => dungeonGame.WasKeyJustPressed(key) || (CanMove && dungeonGame.IsKeyHeldLong(key));

        if (keyPress(Keys.Left))
            moveBuffer.Push(Keys.Left);
        else if (keyPress(Keys.Right))
            moveBuffer.Push(Keys.Right);
        else if (keyPress(Keys.Down))
            moveBuffer.Push(Keys.Down);
        else if (keyPress(Keys.Up))
            moveBuffer.Push(Keys.Up);

        if (CanMove && moveBuffer.Count > 0)
        {
            var moveDelta = Vector2.Zero;
            switch (moveBuffer.Pop())
            {
                case Keys.Left:
                    moveDelta = new(-1, 0);
                    Direction = Left;
                    break;
                case Keys.Right:
                    moveDelta = new(1, 0);
                    Direction = Right;
                    break;
                case Keys.Down:
                    moveDelta = new(0, 1);
                    Direction = Up;
                    break;
                case Keys.Up:
                    moveDelta = new(0, -1);
                    Direction = Down;
                    break;
            }

            this.AddTween(p => p.AnimationRotation, Direction == Left ? -1 : 1, .075f,
                onEnd: _ => this.AddTween(p => p.AnimationRotation, 0, .075f));

            if (moveDelta != Vector2.Zero)
            {
                CanMove = false;

                var newTilePosition = Position / 16 + moveDelta;
                var tile = dungeonGame.GetDungeonTile((int)newTilePosition.X, (int)newTilePosition.Y);
                if (tile >= Tile.BLOCKING_START)
                {
                    moveDelta = Vector2.Zero;
                    moveBuffer.Clear();
                    CanMove = true;
                } else
                    TweenToPosition(Position + moveDelta * 16, .2f, onEnd: _ => CanMove = true);
            }
        }

        (Rotation, Effects) = Direction switch
        {
            Up => (MathF.PI/2, SpriteEffects.None),
            Down => (-MathF.PI/2, SpriteEffects.None),
            Left => (0, SpriteEffects.FlipHorizontally),
            _ => (0, SpriteEffects.None),
        };

        RebuildSightMap(dungeonGame.DungeonSize, dungeonGame.DungeonSize);
    }

    public bool CanSee(int i, int j)
        => ParentMap.CellIsInBounds(i, j) ? !IsUnderground || SightMap[i, j] : false;

    public bool CanSee(Vector2 worldPosition)
    {
        if (ParentMap.WorldIsInBounds(worldPosition))
            return !IsUnderground || SightMap[(int)worldPosition.X / ParentMap.TileWidth, (int)worldPosition.Y / ParentMap.TileHeight];
        else
            return false;
    }
}
