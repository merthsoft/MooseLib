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
    public int SelectedSpell;

    public MiniMapTile[,] MiniMap = new MiniMapTile[0, 0];

    public string Name = "Wizz";
    public int Health = 1;
    public int Armor = 0;
    public int MagicArmor = 0;
    public int Mana = 10;
    public int MaxMana = 10;
    public int Gold = 0;
    public int DungeonLevel = 1;

    public bool StatsUpdated;
    public bool ItemsUpdated;

    public List<DungeonItem> Items = new();

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

    public void NewFloor()
    {
        Health = 1;
        Mana = 10;
        DungeonLevel++;
        StatsUpdated = true;
        Items.Clear(); // TODO: Don't keep this
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
        var (x, y) = ((int)position.X / 16, (int)position.Y / 16);
        if (game.GetMonsterTile(x, y) > MonsterTile.None && CanMove && CanSee(x, y) == FogOfWar.None)
        { 
            position += new Vector2(8, 8);
            spriteBatch.Draw(game.CrosshairTexture, position, null,
                CrosshairColor, CrosshairRotation,
                game.CrosshairOrigin, CrosshairScale,
                SpriteEffects.None, 1f);
        } else if (!game.GetDungeonTile(x, y).IsBlocking() && CanMove && CanSee(x, y) != FogOfWar.Full)
        {
            spriteBatch.FillRect(position, 16, 16, Color.Cyan with { A = 100 });
            var path = ParentMap.FindCellPath(GetCell(), new((int)game.WorldMouse.X / 16, (int)game.WorldMouse.Y / 16));
            foreach (var p in path)               
                spriteBatch.FillRectangle(p.X * 16, p.Y * 16, 16, 16, Color.Orange.HalveAlphaChannel());
        }

        if (VisibleMonsters.Any())
        {
            var monster = VisibleMonsters.First();
            var monsterPosition = monster.Position + new Vector2(8, 8) + monster.AnimationPosition;
            spriteBatch.Draw(game.CrosshairTexture, monsterPosition, null,
                CrosshairColor, CrosshairRotation,
                game.CrosshairOrigin, CrosshairScale,
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

    public override void Update(MooseGame mooseGame, GameTime gameTime)
    {
        base.Update(mooseGame, gameTime);

        if (!game.CanPlay)
            return;

        HasInputThisFrame = false;

        bool keyPress(Keys key) => CanMove && game.IsKeyDown(key);

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
        } else if (keyPress(Keys.Space) && CanMove)
        {
            if (VisibleMonsters.Any())
                game.Cast(KnownSpells[SelectedSpell], this, VisibleMonsters[0].Position + new Vector2(8, 8));
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
            ProcessMove(move);
        }

        CalculateDirection();

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
            if (game.WasRightMouseJustPressed() || game.GetMonsterTile(x, y) > MonsterTile.None)
            {
                game.Cast(KnownSpells[SelectedSpell], this, game.WorldMouse);
            }
            else if (game.WasLeftMouseJustPressed() && !game.GetDungeonTile(x, y).IsBlocking() && CanSee(x, y) != FogOfWar.Full)
            {
                var path = ParentMap.FindCellPath(GetCell(), new((int)game.WorldMouse.X / 16, (int)game.WorldMouse.Y / 16));
                if (path.Any())
                {
                    var lastCell = GetCell();
                    Direction = null;
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

    private void ProcessMove(string move)
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
                if (moveBuffer.Any())
                {
                    HasInputThisFrame = false;
                    moveBuffer.Clear();
                }
                CanMove = true;
                mouseBuffer = false;
            }
            else
            {
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
