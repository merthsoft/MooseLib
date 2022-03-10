using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Entities;

public record DungeonPlayerDef : DungeonCreatureDef
{
    public DungeonPlayerDef() : base("player", "player", "Heroes")
    {
        DefaultLayer = "player";
        HitPoints = 1;
    }
}

public class DungeonPlayer : DungeonCreature
{
    public override int DrawIndex => 22;

    public bool CanMove { get; set; } = true;
    public bool HasInputThisFrame { get; set; }
    public bool HasActiveSpells { get; set; }
    
    private readonly Queue<string> moveBuffer = new();
    private bool mouseBuffer = false;

    public float CrosshairRotation { get; set; }
    public Color CrosshairColor => ColorHelper.FromHsl(CrosshairHue, 1, .5f);
    public float CrosshairScale { get; set; }

    public float CrosshairHue { get; set; }

    public List<SpellDef> KnownSpells { get; } = new();
    public int SelectedSpell { get; set; }

    public MiniMapTile[,] MiniMap { get; set; } = new MiniMapTile[0, 0];

    public DungeonPlayer(DungeonPlayerDef def) : base(def, Vector2.Zero, Up, layer: "player")
    {
        StartCursorTween();
        UseVisionCircle = true;
    }

    protected override void SetTileVisible(DungeonGame dungeonGame, int x, int y)
    {
        base.SetTileVisible(dungeonGame, x, y);
        MiniMap[x, y] = dungeonGame.GetMiniMapTile(x, y);
    }

    public void Reset(DungeonGame dungeonGame)
    {
        VisibleMonsters.Clear();
        SightMap = new bool[dungeonGame.DungeonSize, dungeonGame.DungeonSize];
        MiniMap = new MiniMapTile[dungeonGame.DungeonSize, dungeonGame.DungeonSize];
        for (var i = 0; i < dungeonGame.DungeonSize; i++)
            for (var j = 0; j < dungeonGame.DungeonSize; j++)
                MiniMap[i, j] = MiniMapTile.None;

        Position = Vector2.Zero;
        UseVisionCircle = true;
    }

    public void DrawCursor(MooseGame mooseGame, Vector2 position, SpriteBatch spriteBatch)
    {
        var dungeonGame = (mooseGame as DungeonGame)!;
        
        var (x, y) = ((int)position.X / 16, (int)position.Y / 16);
        if (dungeonGame.GetMonsterTile(x, y) > MonsterTile.None && CanMove && CanSee(x, y))
        { 
            position += new Vector2(8, 8);
            spriteBatch.Draw(dungeonGame.Crosshair, position, null,
                CrosshairColor, CrosshairRotation,
                dungeonGame.CrosshairOrigin, CrosshairScale,
                SpriteEffects.None, 1f);
        } else if (!dungeonGame.GetDungeonTile(x, y).IsBlocking() && CanMove && CanSee(x, y))
        {
            spriteBatch.FillRect(position, 16, 16, Color.Cyan with { A = 100 });
            var path = ParentMap.FindCellPath(GetCell(), new((int)dungeonGame.WorldMouse.X / 16, (int)dungeonGame.WorldMouse.Y / 16));
            foreach (var p in path)               
                spriteBatch.FillRectangle(p.X * 16, p.Y * 16, 16, 16, Color.Orange.HalveAlphaChannel());
        }

        if (VisibleMonsters.Any())
        {
            var monster = VisibleMonsters.First();
            var monsterPosition = monster.Position + new Vector2(8, 8) + monster.AnimationPosition;
            spriteBatch.Draw(dungeonGame.Crosshair, monsterPosition, null,
                CrosshairColor, CrosshairRotation,
                dungeonGame.CrosshairOrigin, CrosshairScale,
                SpriteEffects.None, 1f);
        }
    }

    private void StartCursorTween()
    {
        CrosshairRotation = 0;
        CrosshairHue = 0;
        CrosshairScale = 1.5f;
        //this.AddTween(p => p.CrosshairRotation, MathF.PI * 2, 1, repeatCount: -1);
        this.AddTween(p => p.CrosshairScale, 2, 1, repeatCount: -1, autoReverse: true);
        this.AddTween(p => p.CrosshairHue, 1, 5, repeatCount: -1);
    }

    public override void AddSpell(Spell spell)
    {
        base.AddSpell(spell);
        CanMove = false;
        HasActiveSpells = true;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
        var dungeonGame = (game as DungeonGame)!;

        if (!dungeonGame.CanPlay)
            return;

        HasInputThisFrame = false;

        if (HasActiveSpells && !ActiveSpells.Any())
        {
            HasInputThisFrame = true;
            CanMove = true;
            HasActiveSpells = false;
        }

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
        } else if (keyPress(Keys.Space) && CanMove)
        {
            if (VisibleMonsters.Any())
                dungeonGame.Cast(KnownSpells.First(), this, VisibleMonsters[0].Position + new Vector2(8, 8));
            keyPressed = true;
        }

        if (keyPressed && mouseBuffer && moveBuffer.Any())
        {
            var last = moveBuffer.Last();
            moveBuffer.Clear();
            moveBuffer.Enqueue(last);
            mouseBuffer = false;
        }

        if (CanMove && moveBuffer.Count > 0)
        {
            var move = moveBuffer.Dequeue();
            ProcessMove(dungeonGame, move);
        }

        CalculateDirection();

        RebuildSightMap(dungeonGame);

        if (CanMove && (dungeonGame.WasLeftMouseJustPressed() || dungeonGame.WasRightMouseJustPressed()))
        {
            var mouse = new Vector2((int)game.WorldMouse.X / 16 * 16, (int)game.WorldMouse.Y / 16 * 16);
            var (x, y) = ((int)mouse.X / 16, (int)mouse.Y / 16);
            if (dungeonGame.WasRightMouseJustPressed() || dungeonGame.GetMonsterTile(x, y) > MonsterTile.None)
            {
                dungeonGame.Cast(KnownSpells.First(), this, dungeonGame.WorldMouse);
            }
            else if (dungeonGame.WasLeftMouseJustPressed() && !dungeonGame.GetDungeonTile(x, y).IsBlocking() && CanSee(x, y))
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
                        var dir = DirectionFrom(diff);
                        Direction ??= dir;
                        if (dir != null)
                            moveBuffer.Enqueue(dir);
                    }
                    mouseBuffer = true;
                }
            }
        }
    }

    private void ProcessMove(DungeonGame dungeonGame, string move)
    {
        HasInputThisFrame = true;
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

        if (newDirection != Direction && !mouseBuffer)
        {
            moveBuffer.Clear();
            mouseBuffer = false;
        }

        Direction = newDirection;

        this.AddTween(p => p.AnimationRotation, Direction == Left ? -1 : 1, .075f,
            onEnd: _ => this.AddTween(p => p.AnimationRotation, 0, .075f));

        if (CanMove && moveDelta != Vector2.Zero)
        {
            CanMove = false;

            var newTilePosition = Position / 16 + moveDelta;
            var tile = dungeonGame.GetDungeonTile((int)newTilePosition.X, (int)newTilePosition.Y);
            if (tile.IsBlocking())
            {
                moveDelta = Vector2.Zero;
                if (moveBuffer.Any())
                {
                    HasInputThisFrame = false;
                    moveBuffer.Clear();
                }
                CanMove = true;
                mouseBuffer = false;
            }
            else {
                AnimationPosition = Vector2.Zero;
                moveDelta *= 16;
                this.AddTween(p => p.AnimationPosition, moveDelta, .2f,
                    onEnd: _ =>
                    {
                        CanMove = true;
                        Position += moveDelta;
                        AnimationPosition = Vector2.Zero;
                    });
            }
        }
    }

    public void LearnSpell(SpellDef spellDef)
        => KnownSpells.Add(spellDef);
}
