using Merthsoft.Moose.Dungeon.Entities.Items;
using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities;

public record DungeonPlayerDef : DungeonCreatureDef
{
    public DungeonPlayerDef() : base("player", "player", "Heroes")
    {
        DefaultLayer = "player";
        HitPoints = 1;
        MiniMapTile = MiniMapTile.Player;
    }
}

public class DungeonPlayer : DungeonCreature
{
    public static DungeonPlayer Instance { get; private set; } = null!;

    public bool CanMove = true;
    public bool HasInputThisFrame;
    
    private readonly Queue<string> moveBuffer = new();
    private bool mouseBuffer = false;

    public float CrosshairRotation;
    public Color CrosshairColor => ColorHelper.FromHsl(CrosshairHue, 1, .5f);
    public float CrosshairScale;

    public float CrosshairHue;

    public List<SpellDef> KnownSpells = new();
    public int SelectedSpellIndex;

    public SpellDef SelectedSpell => KnownSpells[SelectedSpellIndex];

    public MiniMapTile[,] MiniMap = new MiniMapTile[0, 0];

    public string Name = "Wizz";

    public int ManaHealRate = 20;
    public int ManaHealCount = 0;
    public int Mana = 0;
    public int MaxMana = 10;
    public int Gold = 0;
    public int DungeonLevel = 1;
    public int NumSteps = 0;
    public int NumMoves = 0;
    public int NumKills = 0;

    public bool StatsUpdated;
    public bool ItemsUpdated;

    public List<DungeonItem> Items = new();

    public bool Targeting = false;
    public int TargetIndex = -1;
    public Vector2 TargetPosition;
    //public Vector2 TargetAnimationPosition;

    public DungeonPlayer(DungeonPlayerDef def) : base(def, Vector2.Zero, Up, layer: "player")
    {
        Instance = this;

        StartCursorTween();
        UseVisionCircle = true;
        DrawIndex = 22;
        BuildSightMap = true;
    }

    protected override void SetTileVisible(DungeonGame dungeonGame, int x, int y)
    {
        base.SetTileVisible(dungeonGame, x, y);
        MiniMap[x, y] = dungeonGame.GetMiniMapTile(x, y);
    }

    protected override void CellDiscovered(int x, int y)
    {
        base.CellDiscovered(x, y);
        if (DungeonLevel > 0 && Mana < MaxMana)
        {
            ManaHealCount++;
            if (ManaHealCount == ManaHealRate)
            {
                ManaHealCount = 0;
                HealMp(1);
            }
        }
    }

    public void NewFloor()
    {
        this.ClearTweens();
        State = Alive;
        HitPoints = 1;
        Mana = 10000;
        DungeonLevel++;
        StatsUpdated = true;
        TargetIndex = -1;
        Targeting = false;
        CanMove = true;
        Armor = 10;
        MagicArmor = 10;
        Items.Clear(); // TODO: Don't keep this

        AnimationPosition = Vector2.Zero;
        AnimationRotation = 0;
        Rotation = 0;
        Scale = Vector2.One;
        Color = Color.White;
        Direction = Right;
        CurrentlyBlockingInput = false;

        StatsUpdated = true;
        ItemsUpdated = true;
    }

    public override void ResetVision()
    {
        base.ResetVision();

        MiniMap = new MiniMapTile[game.DungeonSize, game.DungeonSize];
        for (var i = 0; i < game.DungeonSize; i++)
            for (var j = 0; j < game.DungeonSize; j++)
                MiniMap[i, j] = MiniMapTile.None;
    }

    public void DrawCursor(MooseGame mooseGame, Vector2 position, SpriteBatch spriteBatch)
    {
        if (Targeting)
        {
            for (var i = 0; i < VisibleMonsters.Count; i++)
            {
                var pos = VisibleMonsters[i].Position + new Vector2(8, 8);
                spriteBatch.Draw(game.CrosshairTexture, pos, null,
                    i == TargetIndex ? CrosshairColor : Color.White, i == TargetIndex ? CrosshairRotation : 0,
                    game.CrosshairOrigin, i == TargetIndex ? CrosshairScale : 0,
                    SpriteEffects.None, 1f);
            }

            if (TargetIndex == -1)
                spriteBatch.Draw(game.CrosshairTexture, TargetPosition + new Vector2(8, 8), null,
                    CrosshairColor,CrosshairRotation,
                    game.CrosshairOrigin, CrosshairScale,
                    SpriteEffects.None, 1f);
        } else
        {
            var (x, y) = ((int)position.X / 16, (int)position.Y / 16);
            if (CanMove && CanSee(x, y) != FogOfWar.Full)
            {
                spriteBatch.FillRect(position, 16, 16, Color.Cyan with { A = 100 });
                var path = ParentMap.FindCellPath(GetCell(), new((int)game.WorldMouse.X / 16, (int)game.WorldMouse.Y / 16));
                foreach (var p in path)
                    spriteBatch.FillRectangle(p.X * 16, p.Y * 16, 16, 16, Color.Orange.HalveAlphaChannel());
            }
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

    private void TargetStateUpdate(GameTime gameTime)
    {
        if (TargetIndex != -1)
            TargetPosition = VisibleMonsters[TargetIndex].Position;

        if (game.WasKeyJustPressed(Keys.Space) && CanMove)
        {
            game.Cast(SelectedSpell, this, TargetPosition);
            Targeting = false;
            HasInputThisFrame = true;
            return;
        }

        var direction = "";
        if (keyPress(Keys.Left))
            direction = Left;
        else if (keyPress(Keys.Right))
            direction = Right;
        else if (keyPress(Keys.Down))
            direction = Down;
        else if (keyPress(Keys.Up))
            direction = Up;
        else if (keyPress(Keys.Escape, Keys.Q))
            Targeting = false;

        if (direction != "")
        {
            if (TargetIndex != -1)
                TargetIndex = -1;
            CanMove = false;

            if (SelectedSpell.TargetMode == TargetMode.Free)
            {
                var newPosition = TargetPosition + direction.GetDelta() * 16;
                this.AddTween(t => t.TargetPosition, newPosition, .15f,
                    onEnd: _ => CanMove = true);
            }
            else if (SelectedSpell.TargetMode == TargetMode.FourWay)
            {
                var newPosition = Position + direction.GetDelta() * 16;
                this.AddTween(t => t.TargetPosition, newPosition, .15f,
                    onEnd: _ => CanMove = true);
            }
        }

        if (game.MouseInGame && CanMove && (game.WasLeftMouseJustPressed() || game.WasRightMouseJustPressed()))
        {
            var mouse = new Vector2((int)game.WorldMouse.X / 16 * 16, (int)game.WorldMouse.Y / 16 * 16);
            var (x, y) = ((int)mouse.X / 16, (int)mouse.Y / 16);
            var fow = CanSee(x, y);
            if (game.WasLeftMouseJustPressed() && fow == FogOfWar.None)
            {
                var monster = game.GetMonster(x, y);
                if (monster != null)
                {
                    if (monster.Position != TargetPosition)
                        Target(monster);
                    else
                    {
                        game.Cast(SelectedSpell, this, TargetPosition);
                        Targeting = false;
                        return;
                    }
                }
            }
        }
    }

    bool keyPress(params Keys[] key) => CanMove && game.IsKeyDown(key);

    public override void Update(MooseGame mooseGame, GameTime gameTime)
    {
        base.Update(mooseGame, gameTime);

        if (Remove)
        {
            Remove = false;
            return;
        }
        HasInputThisFrame = false;

        if (!game.CanPlay)
            return;

        if (Targeting)
        {
            TargetStateUpdate(gameTime);
            return;
        }

        var keyPressed = false;
        if (keyPress(Keys.Left))
        {
            moveBuffer.Clear();
            moveBuffer.Enqueue(Left);
            keyPressed = true;
        }
        else if (keyPress(Keys.Right))
        {
            moveBuffer.Clear();
            moveBuffer.Enqueue(Right);
            keyPressed = true;
        }
        else if (keyPress(Keys.Down))
        {
            moveBuffer.Clear();
            moveBuffer.Enqueue(Down);
            keyPressed = true;
        }
        else if (keyPress(Keys.Up))
        {
            moveBuffer.Clear();
            moveBuffer.Enqueue(Up);
            keyPressed = true;
        }
        else if (game.WasKeyJustPressed(Keys.Space) && CanMove && !Targeting)
            Target(VisibleMonsters.OrderBy(m => m.DistanceSquaredTo(Position)).FirstOrDefault());
        else if (keyPress(Keys.End))
            HasInputThisFrame = true;

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
            ProcessMove(move);
        }

        Rotation = CalculateRotation(Direction);

        RebuildSightMap(game);

        if (VisibleMonsters.Count > 0)
        {
            moveBuffer.Clear();
            mouseBuffer = false;
        }

        if (game.MouseInGame && CanMove && (game.WasLeftMouseJustPressed() || game.WasRightMouseJustPressed()))
        {
            var mouse = new Vector2((int)mooseGame.WorldMouse.X / 16 * 16, (int)mooseGame.WorldMouse.Y / 16 * 16);
            var (x, y) = ((int)mouse.X / 16, (int)mouse.Y / 16);
            if (game.WasLeftMouseJustPressed() && !game.GetDungeonTile(x, y).IsBlocking() && CanSee(x, y) != FogOfWar.Full)
            {
                var monster = game.GetMonster(x, y);
                if (monster != null)
                    Target(monster);
                else
                {
                    var path = ParentMap.FindCellPath(GetCell(), new((int)game.WorldMouse.X / 16, (int)game.WorldMouse.Y / 16));
                    if (path.Any())
                    {
                        var lastCell = GetCell();
                        Direction = null!;
                        foreach (var p in path)
                        {
                            if (CanSee(p.X, p.Y) == FogOfWar.Full)
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

        if (HasInputThisFrame)
            NumMoves++;
    }

    private void Target(DungeonCreature? monster)
    {
        StartCursorTween();
        Targeting = true;
        var spellMode = SelectedSpell.TargetMode;
        switch (spellMode)
        {
            case TargetMode.Free:
                if (monster == null)
                    goto default;
                var index = VisibleMonsters.IndexOf(monster);
                if (index == -1)
                    goto default;
                TargetIndex = index;
                break;
            case TargetMode.FourWay:
                TargetIndex = -1;
                TargetPosition = Position + Direction.GetDelta() * 16;
                break;
            default:
                TargetIndex = -1;
                TargetPosition = Position;
                break;

        };
    }

    protected override void ProcessMove(string move)
    {
        var moveDelta = move.GetDelta();
        var newDirection = move;

        if (newDirection != Direction && !mouseBuffer)
        {
            moveBuffer.Clear();
            mouseBuffer = false;
        }

        Direction = newDirection;

        this.AddTween(p => p.AnimationRotation, Direction == Left ? 1 : -1, .05f,
            onEnd: _ => this.AddTween(p => p.AnimationRotation, 0, .07f));

        if (CanMove && moveDelta != Vector2.Zero && !game.IsKeyDown(Keys.LeftShift, Keys.RightShift))
        {
            HasInputThisFrame = true;
            CanMove = false;

            var newTilePosition = Position / 16 + moveDelta;
            var newX = (int)newTilePosition.X;
            int newY = (int)newTilePosition.Y;
            var tile = game.GetDungeonTile(newX, newY);
            var monster = game.GetMonster(newX, newY);
            var item = game.GetItem(newX, newY);
            if (tile.IsBlocking() || monster != null || (item != null && item.ItemDef.BlocksPlayer))
            {
                moveDelta = Vector2.Zero;
                HasInputThisFrame = false;
                if (moveBuffer.Any())
                {
                    moveBuffer.Clear();
                }
                CanMove = true;
                mouseBuffer = false;
            }
            else
            {
                moveDelta *= 16;
                Position += moveDelta;
                AnimationPosition = -moveDelta;
                this.AddTween(p => p.AnimationPosition, Vector2.Zero, .2f,
                    onEnd: _ =>
                    {
                        CanMove = true;
                        AnimationPosition = Vector2.Zero;
                    });
                NumSteps++;
            }

            if (item != null && item is InteractiveItem interactiveItem && !interactiveItem.InteractedWith)
                interactiveItem.Interact();
        }
    }

    public void LearnSpell(SpellDef spellDef)
        => KnownSpells.Add(spellDef);

    public MiniMapTile GetMiniMapTile(int i, int j)
    {
        if (!UseVisionCircle)
            return game.GetMiniMapTile(i, j);
        var (x, y) = GetCell();
        if (x == i && y == j)
            return MiniMapTile.Player;

        return MiniMap[i, j];
    }

    public void GiveGold(Vector2 position, int value)
    {
        Gold += value;
        game.SpawnFallingText(value.ToString(), position, Color.Gold);
        StatsUpdated = true;
    }

    public override void TakeDamage(int value)
    {
        base.TakeDamage(value);
        StatsUpdated = true;
    }

    public bool TrySpendMana(int mana)
    {
        if (mana > Mana)
            return false;
        Mana -= mana;
        StatsUpdated = true;
        
        return true;
    }

    public void GiveItem(DungeonItem item)
    {
        ItemsUpdated = true;
        Items.Add(item);
    }

    public void TakeItem(DungeonItem item)
    {
        ItemsUpdated = true;
        Items.Remove(item);
    }

    public virtual void HealMp(int value, bool overHeal = false)
    {
        StatsUpdated = true;
        Mana += value;
        var total = value;
        if (!overHeal && Mana > MaxMana)
        {
            total -= Mana - MaxMana;
            Mana = MaxMana;
        }
        game.SpawnFallingText($"+{total}", Position, Color.Blue);
    }
}
