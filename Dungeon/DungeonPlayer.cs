using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;

public class DungeonPlayer : GameObjectBase
{
    public const string Left = "Left";
    public const string Right = "Right";
    public const string Up = "Up";
    public const string Down = "Down";

    public bool CanMove { get; set; } = true;
    
    private readonly Queue<string> moveBuffer = new();
    private bool mouseBuffer = false;

    public float AnimationRotation { get; set; }

    public float CrosshairRotation { get; set; }
    public Color CrosshairColor => ColorHelper.FromHsl(CrosshairHue, 1, .5f);
    public float CrosshairScale { get; set; }

    public float CrosshairHue { get; set; }

    private DungeonPlayerDef PlayerDef => (Def as DungeonPlayerDef)!;

    private bool[,] SightMap { get; set; } = new bool[0,0];

    public bool UseVisionCone { get; set; } = true;
    public int ViewRadius { get; set; } = 8;

    public List<SpellDef> KnownSpells { get; } = new();

    public DungeonPlayer(DungeonPlayerDef def) : base(def)
    {
        StartCursorTween();
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(PlayerDef.Texture, WorldRectangle.Move(new(8, 8)).ToRectangle(), new(64, 48, 16, 16), Color.White, Rotation + AnimationRotation, new(8, 8), Effects, 1f);
        var mouse = new Vector2((int)game.WorldMouse.X / 16 * 16, (int)game.WorldMouse.Y / 16 * 16);
        DrawCursor(game, mouse, spriteBatch);
    }

    public virtual void DrawCursor(MooseGame mooseGame, Vector2 position, SpriteBatch spriteBatch)
    {
        var dungeonGame = (mooseGame as DungeonGame)!;
        spriteBatch.FillRect(position, 16, 16, Color.Cyan with { A = 100 });
        var (x, y) = ((int)position.X / 16, (int)position.Y / 16);
        if (dungeonGame.GetMonsterTile(x, y) > MonsterTile.None)
        { 
            position += new Vector2(8, 8);
            spriteBatch.Draw(dungeonGame.Crosshair, position, null,
                CrosshairColor, CrosshairRotation,
                dungeonGame.CrosshairOrigin, CrosshairScale,
                SpriteEffects.None, 1f);
        } else if (!dungeonGame.GetDungeonTile(x, y).IsBlocking() && CanMove && CanSee(x, y))
        {
            var path = ParentMap.FindCellPath(GetCell(), new((int)dungeonGame.WorldMouse.X / 16, (int)dungeonGame.WorldMouse.Y / 16));
            foreach (var p in path)               
                spriteBatch.FillRectangle(p.X * 16, p.Y * 16, 16, 16, Color.Orange.HalveAlphaChannel());
        }
    }

    public void RebuildSightMap(DungeonGame dungeonGame)
    {
        var (dungeonWidth, dungeonHeight) = (dungeonGame.DungeonSize, dungeonGame.DungeonSize);
        SightMap = new bool[dungeonWidth, dungeonHeight];
        var cell = GetCell();
        var (playerX, playerY) = cell;
        SightMap[playerX, playerY] = true;

        for (var d = 0f; d < MathF.PI * 2; d+=.01f)
            for (var delta = 1f; delta < ViewRadius; delta+=1)
            {
                var posX = (playerX + delta * MathF.Cos(d)).Round();
                var posY = (playerY + delta * MathF.Sin(d)).Round();

                if (posX < 0 || posY < 0 || posX >= dungeonWidth || posY >= dungeonHeight)
                    continue;

                SightMap[posX, posY] = true;
                if (dungeonGame.GetDungeonTile(posX, posY).BlocksSight())
                    break;
            }
    }

    private void StartCursorTween()
    {
        CrosshairRotation = 0;
        CrosshairHue = 0;
        CrosshairScale = 1;
        //this.AddTween(p => p.CrosshairRotation, MathF.PI * 2, 1, repeatCount: -1);
        this.AddTween(p => p.CrosshairScale, 1.5f, 1, repeatCount: -1, autoReverse: true);
        this.AddTween(p => p.CrosshairHue, 1, 5, repeatCount: -1);
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        var dungeonGame = (game as DungeonGame)!;
        bool keyPress(Keys key) => dungeonGame.WasKeyJustPressed(key) || (CanMove && dungeonGame.IsKeyHeldLong(key));

        var keyPressed = false;
        if (keyPress(Keys.Left))
        {
            moveBuffer.Enqueue(Left);
            keyPressed = true;
        }
        else if (keyPress(Keys.Right))
        { 
            moveBuffer.Enqueue(Right);
            keyPressed = true;
        }
        else if (keyPress(Keys.Down))
        { 
            moveBuffer.Enqueue(Down);
            keyPressed = true;
        }
        else if (keyPress(Keys.Up))
        {
            moveBuffer.Enqueue(Up);
            keyPressed = true;
        }

        if (keyPressed && mouseBuffer)
        {
            var last = moveBuffer.Last();
            moveBuffer.Clear();
            moveBuffer.Enqueue(last);
            mouseBuffer = false;
        }

        if (CanMove && moveBuffer.Count > 0)
        {
            var moveDelta = Vector2.Zero;
            var newDirection = "";
            switch (moveBuffer.Dequeue())
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

            if (newDirection != Direction && !mouseBuffer)
            {
                moveBuffer.Clear();
                mouseBuffer = false;
            }

            Direction = newDirection;

            this.AddTween(p => p.AnimationRotation, Direction == Left ? -1 : 1, .075f,
                onEnd: _ => this.AddTween(p => p.AnimationRotation, 0, .075f));

            if (moveDelta != Vector2.Zero)
            {
                CanMove = false;

                var newTilePosition = Position / 16 + moveDelta;
                var tile = dungeonGame.GetDungeonTile((int)newTilePosition.X, (int)newTilePosition.Y);
                if (tile.IsBlocking())
                {
                    moveDelta = Vector2.Zero;
                    moveBuffer.Clear();
                    CanMove = true;
                    mouseBuffer = false;
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

        RebuildSightMap(dungeonGame);

        if (CanMove && dungeonGame.WasLeftMouseJustPressed())
        {
            var mouse = new Vector2((int)game.WorldMouse.X / 16 * 16, (int)game.WorldMouse.Y / 16 * 16);
            var (x, y) = ((int)mouse.X / 16, (int)mouse.Y / 16);
            if (dungeonGame.GetMonsterTile(x, y) > MonsterTile.None)
                dungeonGame.Cast(KnownSpells.First(), this, dungeonGame.WorldMouse);
            else if (!dungeonGame.GetDungeonTile(x, y).IsBlocking() && CanSee(x, y))
            {
                var path = ParentMap.FindCellPath(GetCell(), new((int)dungeonGame.WorldMouse.X / 16, (int)dungeonGame.WorldMouse.Y / 16));
                if (path.Any())
                {
                    var lastCell = GetCell();
                    Direction = null;
                    foreach (var p in path)
                    {
                        if (!CanSee(p.X, p.Y))
                            break;
                        
                        var diff = lastCell - p;
                        lastCell = p;
                        var dir = (diff.X, diff.Y) switch
                        {
                            (-1, 0) => Right,
                            (1, 0) => Left,
                            (0, -1) => Down,
                            (0, 1) => Up,
                            _ => null,
                        };
                        Direction ??= dir;
                        if (dir != null)
                            moveBuffer.Enqueue(dir);
                    }
                    mouseBuffer = true;
                }
            }
        }
    }

    public bool CanSee(int i, int j)
        => ParentMap.CellIsInBounds(i, j) && (!UseVisionCone || SightMap[i, j]);

    public bool CanSee(Vector2 worldPosition)
    {
        if (ParentMap.WorldIsInBounds(worldPosition))
            return !UseVisionCone || SightMap[(int)worldPosition.X / ParentMap.TileWidth, (int)worldPosition.Y / ParentMap.TileHeight];
        else
            return false;
    }
}
