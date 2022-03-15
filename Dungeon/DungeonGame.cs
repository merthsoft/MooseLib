using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Items;
using Merthsoft.Moose.Dungeon.Entities.Monsters;
using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.Dungeon.Map;
using Merthsoft.Moose.Dungeon.Renderers;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.Dungeon.Ux;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.Dungeon;

public class DungeonGame : MooseGame
{
    public static new DungeonGame Instance = null!;

    public SpellBook SpellBook = new();
    public Dictionary<MonsterTile, (MonsterDef, Func<MonsterDef, int, int, DungeonMonster>)> MonsterFactory = new();
    public Dictionary<ItemTile, (ItemDef, Func<ItemDef, int, int, DungeonItem>)> ItemFactory = new();

    public Texture2D DungeonTiles = null!;
    public Texture2D MiniMapTiles = null!;
    public Texture2D CrosshairTexture = null!;
    public Vector2 CrosshairOrigin;

    private Texture2D CursorTexture = null!;

    public Texture2D MapTexture = null!;
    public Texture2D MapCornerTexture = null!;

    public Texture2D NextButtonTexture = null!;
    public Texture2D PreviousButtonTexture = null!;

    public Texture2D ItemTiles = null!;

    public Texture2D ArrowTexture = null!;

    SpriteFont DebugFont = null!;
    SpriteFont FallingTextFont = null!;

    public int BaseTileWidth = 16;
    public int BaseTileHeight = 16;

    public int DungeonSize = 34;

    public Window UxWindow = null!;

    public MapPanel MapPanel = null!;

    public int ViewingMap = 0;

    public bool GeneratingDungeon = false;

    private readonly (int w, int h)[] roomSizes = new[]
        {
            (5, 5),
            (6, 6),
            (7, 6),
            (7, 4),
            (4, 7),
            (6, 5),
            (5, 6),
        };

    public DungeonMap DungeonMap = null!;

    DungeonPlayerDef PlayerDef = new();
    public DungeonPlayer Player = null!;

    List<FallingText> FallingTexts = new List<FallingText>();

    public bool CanPlay = true;
    public bool MouseInGame => CurrentMouseState.X > 320;

    public DungeonGame()
    {
        Instance = this;
        Player = new(PlayerDef) { };
        IsMouseVisible = false;

        DefaultRenderHooks = new()
        {
            { "items", new(PostHook: _ => DrawCursor()) },
            { "spells", new(PostHook: _ => DrawFallingTexts()) },
        };
    }

    protected override StartupParameters Startup()
        => base.Startup() with
        {
            ScreenWidth = 1280,
            ScreenHeight = 960,
            DefaultBackgroundColor = new(53, 32, 42),
        };

    protected override void Load()
    {
        Window.Title = "Wiggle Wizzard";
        CrosshairTexture = ContentManager.LoadImage("Crosshair");
        CursorTexture = ContentManager.LoadImage("Cursor");
        MiniMapTiles = ContentManager.LoadImage("MiniMap");
        MapTexture = ContentManager.LoadImage("MapWindow");
        MapCornerTexture = ContentManager.LoadImage("MapCorner");

        CrosshairOrigin = new(CrosshairTexture.Width/2, CrosshairTexture.Height/2);

        NextButtonTexture = ContentManager.LoadImage("NextButton");
        PreviousButtonTexture = ContentManager.LoadImage("PreviousButton");

        ItemTiles = ContentManager.LoadImage("Items");

        ArrowTexture = ContentManager.LoadImage("Arrow");

        DungeonTiles = ContentManager.LoadImage("Dungeon");
        var dungeonRenderer = new DungeonRenderer(Player, SpriteBatch, BaseTileWidth, BaseTileHeight, DungeonTiles); 
        
        dungeonRenderer[DungeonTile.StoneWall] = ContentManager.LoadImage("StoneWall");
        dungeonRenderer[DungeonTile.BrickWall] = ContentManager.LoadImage("BrickWall");
        AddDefaultRenderer<DungeonLayer>("dungeon", dungeonRenderer);
        AddDefaultRenderer<ObjectLayer>("objects", new DungeonObjectRenderer(SpriteBatch));

        var miniMapRenderer = new MiniMapRenderer(SpriteBatch, 8, 8, MiniMapTiles);

        DungeonMap = new DungeonMap(DungeonSize, DungeonSize);
        ActiveMaps.Add(DungeonMap);

        MainCamera.Zoom = 3f;
        AddDef(PlayerDef);
        DebugFont = ContentManager.BakeFont("MatchupPro", 30);
        FallingTextFont = ContentManager.BakeFont("Border_Basic_Monospaced", 24);

        AddSpellDef(new FireballDef(), (spellDef, owner, position) => new Fireball(spellDef, owner, position));
        AddSpellDef(new MeteorDef(), (spellDef, owner, position)
            => 
            new SpellContainer(owner)
                .Add(new Meteor(spellDef, owner, position))
                .Add(new Meteor(spellDef, owner, position - new Vector2(16, 0)))
                .Add(new Meteor(spellDef, owner, position - new Vector2(-16, 0)))
                .Add(new Meteor(spellDef, owner, position - new Vector2(0, 16)))
                .Add(new Meteor(spellDef, owner, position - new Vector2(0, -16)))
            );
        AddSpellDef(new SpinesDef(), (spellDef, owner, position) => new Spines(spellDef, owner, position));
        AddSpellDef(new LightningDef(), (spellDef, owner, position) => new Lightning(spellDef, owner, position));
        AddSpellDef(new SpellDef("Dark Shield", 1, "Shield"), (spellDef, owner, position) => new Fireball(spellDef, owner, position));
        AddSpellDef(new SpellDef("AnimateDead", 1, "Raise"), (spellDef, owner, position) => new Fireball(spellDef, owner, position));
        AddSpellDef(new SpellDef("Slow", 1, "Tangle"), (spellDef, owner, position) => new Fireball(spellDef, owner, position));

        Player.LearnSpell(AddSpellDef(new FlameDef(), (spellDef, owner, position) => new Flame(spellDef, owner, position)));

        AddMonsterDef(new MonsterDef("Marshall", MonsterTile.Marshall) { HitPoints = 5 }, 
            (def, x, y) => new Marshall(def, new Vector2(x * 16, y * 16)));

        AddMonsterDef(new MonsterDef("Slime", MonsterTile.BlueSlime) { HitPoints = 2}, 
            (def, x, y) => new Slime(def, new Vector2(x * 16, y * 16)));

        for (var item = ItemTile.TREASURE_START; item < ItemTile.TREASURE_END; item++)
            AddItemDef(new TreasureDef(item, item.ToString().InsertSpacesBeforeCapitalLetters()), (itemDef, x, y) => new Treasure((TreasureDef)itemDef, new Vector2(x * 16, y * 16)));

        AddItemDef(new ChestDef(), (itemDef, x, y) => new Chest((ChestDef)itemDef, new Vector2(x * 16, y * 16)));

        var fonts = new[] {
            ContentManager.BakeFont("BrightLinger", 62),
            ContentManager.BakeFont("BrightLinger", 15),
            ContentManager.BakeFont("BrightLinger_monospace", 25)
        };

        UxWindow = new(
            new(ContentManager.LoadImage("Window"), 32, 32,
                fonts, controlDrawOffset: new(16, 16))
            {
                TextColor = Color.DarkGray,
                TextMouseOverColor = Color.DeepPink,
                SelectedColor = Color.Gold,
                SelectedMouseOverColor = Color.HotPink,
            }, 0, 0, ScreenWidth, ScreenHeight) 
        { BackgroundDrawingMode = BackgroundDrawingMode.None };

        var panel = UxWindow.AddPanel(0, 0, 320, 960);
        var spellBook = panel.AddControlPassThrough(new SpellBookPanel(UxWindow, 0, 0));
        MapPanel = panel.AddControlPassThrough(new MapPanel(miniMapRenderer, UxWindow, 0, spellBook.Height));
        var statsPanel = panel.AddControlPassThrough(new StatsPanel(UxWindow, 0, MapPanel.Height + spellBook.Height));
        panel.AddControl(new ItemsPanel(UxWindow, 0, statsPanel.Y + statsPanel.Height));
    }

    protected override void PostLoad()
    {
        AddObject(Player);
        GenerateTown();
    }

    private void AddItemDef(ItemDef itemDef, Func<ItemDef, int, int, DungeonItem> generator)
    {
        AddDef(itemDef);
        ItemFactory[itemDef.Item] = (itemDef, generator);
    }

    private void AddMonsterDef(MonsterDef monsterDef, Func<MonsterDef, int, int, DungeonMonster> generator)
    {
        AddDef(monsterDef);
        MonsterFactory[monsterDef.Monster] = (monsterDef, generator);
    }

    private SpellDef AddSpellDef(SpellDef spellDef, Func<SpellDef, DungeonObject, Vector2, Spell> generator)
    {
        AddDef(spellDef);
        SpellBook.AddSpell(spellDef, generator);
        return spellDef;
    }

    public MiniMapTile GetMiniMapTile(int x, int y)
    {
        var mapCell = new Point(x, y);
        var (playerX, playerY) = Player.GetCell();

        var obj = ReadObjects.OfType<DungeonObject>().FirstOrDefault(o => o.GetCell() == mapCell);
        if (obj != null)
            return obj.MiniMapTile;

        return (MiniMapTile)DungeonMap.DungeonLayer.GetTileValue(x, y);
    }

    public DungeonTile GetDungeonTile(int x, int y)
        => DungeonMap.DungeonLayer.GetTileValue(x, y);

    public MonsterTile GetMonsterTile(int x, int y)
        => DungeonMap.Monsters.MonsterDef?.Monster ?? MonsterTile.None;

    public DungeonCreature? GetMonster(int x, int y)
        => DungeonMap.MonsterLayer.Objects.FirstOrDefault(o => o.InCell(x, y)) as DungeonMonster;

    public DungeonItem? GetItem(int x, int y)
        => DungeonMap.ItemLayer.Objects.FirstOrDefault(o => o.InCell(x, y)) as DungeonItem;

    public ItemTile GetItemTile(int x, int y)
        => (DungeonMap.ItemLayer.Objects.FirstOrDefault(o => o.InCell(x, y)) as DungeonItem)?.ItemDef.Item ?? ItemTile.None;

    public bool IsCellOccupied(int x, int y)
        => GetDungeonTile(x, y).IsFloor()
        && !ReadObjects.Any(o => o.InCell(x, y));
    
    public DungeonMonster SpawnMonster(MonsterTile monsterTile, int x, int y)
    {
        var (def, generator) = MonsterFactory[monsterTile];
        return AddObject(generator(def, x, y));
    }

    public DungeonItem SpawnItem(ItemTile itemTile, int x, int y)
    {
        var (def, generator) = ItemFactory[itemTile];
        return AddObject(generator(def, x, y));
    }

    public void Cast(SpellDef spellDef, DungeonObject owner, Vector2 position)
    {
        var spell = SpellBook.Cast(spellDef, owner, position);
        
        var container = spell as SpellContainer;
        
        var spellsToAdd = container != null ? container!.Spells.ToArray() : new[] { spell };
        var manaCost = spell.ManaCost;
        
        if (!Player.TrySpendMana(manaCost))
        {
            SpawnFallingText("Low mana", owner.Position, Color.DarkGray);
            return;
        }

        foreach (var childSpell in spellsToAdd)
        {
            AddObject(childSpell);
            Player.AddSpell(childSpell);
        }

        SpawnFallingText($"-{manaCost}", owner.Position, Color.CornflowerBlue);
    }

    public FallingText SpawnFallingText(string text, Vector2 position, Color? color = null)
    {
        var fallingText = new FallingText(text, position, color);

        var x = Random.Next(-1, 2);
        var y = Random.Next(-1, 2);
        Tween(fallingText, t => t.Position, new Vector2(1500 * x, 1500 * y), Random.NextSingle() + 1f,
            onEnd: _ => fallingText.Done = true,
            easingFunction: EasingFunctions.ExponentialIn);
        FallingTexts.Add(fallingText);

        return fallingText;
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        var x = (Player.Position.X + Player.AnimationPosition.X - 800 / MainCamera.Zoom).Round(2);
        var y = (Player.Position.Y + Player.AnimationPosition.Y - 480 / MainCamera.Zoom).Round(2);
        MainCamera.Position = new(x, y);

        if (WasKeyJustPressed(Keys.D))
            GenerateNextDungeon();
        else if (WasKeyJustPressed(Keys.T))
            GenerateTown();
        else if (WasKeyJustPressed(Keys.Escape, Keys.Q))
            ShouldQuit = true;
        else if (WasKeyJustPressed(Keys.M))
        {
            if (ViewingMap == 0)
            {
                ViewingMap++;
                MapPanel.TweenToPosition(MapPanel.ZoomPosition, .5f, onEnd: _ => ViewingMap++);
                MapPanel.AddTween(t => t.Scale, 3, .5f);
            }
            else if (ViewingMap == 2)
            {
                ViewingMap--;
                MapPanel.TweenToPosition(MapPanel.NormalPosition, .5f, onEnd: _ => ViewingMap--);
                MapPanel.AddTween(t => t.Scale, 1, .5f);
            }

        }
        else if (WasKeyJustPressed(Keys.D1) && Player.KnownSpells.Count > 0)
            Player.SelectedSpell = 0;
        else if (WasKeyJustPressed(Keys.D2) && Player.KnownSpells.Count > 1)
            Player.SelectedSpell = 1;
        else if (WasKeyJustPressed(Keys.D3) && Player.KnownSpells.Count > 2)
            Player.SelectedSpell = 2;
        else if (WasKeyJustPressed(Keys.D4) && Player.KnownSpells.Count > 3)
            Player.SelectedSpell = 3;
        else if (WasKeyJustPressed(Keys.D5) && Player.KnownSpells.Count > 4)
            Player.SelectedSpell = 4;
        else if (WasKeyJustPressed(Keys.D6) && Player.KnownSpells.Count > 5)
            Player.SelectedSpell = 5;
        else if (WasKeyJustPressed(Keys.Z))
        {
            if (MainCamera.Zoom == 3)
                Tweener.TweenTo(MainCamera, m => m.Zoom, 2, .5f);
            else if (MainCamera.Zoom == 2)
                Tweener.TweenTo(MainCamera, m => m.Zoom, 3, .5f);
        }
    }

    protected override void PostUpdate(GameTime gameTime)
    {
        base.PostUpdate(gameTime);

        FallingTexts.RemoveAll(text => text.Done || text.Age++ > 1500);

        var (playerX, playerY) = Player.GetCell();
        if (!GeneratingDungeon && GetDungeonTile(playerX, playerY) == DungeonTile.StairsDown)
        {
            GeneratingDungeon = true;
            UxWindow.OverlayColor = Color.Black;
            UxWindow.OverlayAlpha = 0;

            UxWindow.TweenToOverlayAlpha(1f, .5f, onEnd: _ =>
            {
                GenerateNextDungeon();
                UxWindow.TweenToOverlayAlpha(0, .5f,
                    onEnd: _ => GeneratingDungeon = false);
            });
        }

        CanPlay = !GeneratingDungeon && ViewingMap == 0 && !ReadObjects.OfType<DungeonObject>().Any(o => o.CurrentlyBlockingInput);
        UxWindow.Update(gameTime);
    }

    private void GenerateTown()
    {
        DungeonMap.GenerateTown(Random.Next(4, 8), roomSizes);
        Player.NewFloor();
        var stairRoom = DungeonMap.Rooms.Last();
        Player.UseVisionCircle = false;
        Player.DungeonLevel = 0;
    }

    private void GenerateNextDungeon()
    {
        DungeonMap.GenerateDungeon();
        Player.NewFloor();
        Player.UseVisionCircle = true;

        RemoveDefs<PotionDef>();

        var potionTiles = Enumerable.Range((int)ItemTile.POTION_START, (int)ItemTile.POTION_END).Shuffle();
        var potion = (ItemTile)potionTiles.First();
        AddItemDef(new PotionDef(potion, "Restore Magic"), (def, x, y) => new RestoreMagicPotion(potion, (UsableItemDef)def, new(x * 16, y * 16)));
        Player.GiveItem(SpawnItem(potion, -1000, -1000));
    }

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        UxWindow.Draw(SpriteBatch, gameTime);

        SpriteBatch.DrawString(DebugFont,
            $"FPS: {FramesPerSecondCounter.FramesPerSecond} - Seed: {DungeonMap.SeedUsed}",
            new(325, 920), DefaultBackgroundColor.Shade(3f));

        SpriteBatch.Draw(CursorTexture, CurrentMouseState.Position.ToVector2(), null, Color.White, 0, Vector2.Zero, 3, SpriteEffects.None, 1);
        SpriteBatch.End();
    }

    private void DrawFallingTexts()
    {
        foreach (var fallingText in FallingTexts)
        {
            var stringSize = FallingTextFont.MeasureString(fallingText.Text);
            SpriteBatch.DrawString(FallingTextFont, fallingText.Text,
                fallingText.Position, fallingText.Color, fallingText.Rotation,
                stringSize / 2, fallingText.Scale, fallingText.Effects, 1f);
        }
    }

    void DrawCursor()
    {
        if (CanPlay && Player.CanMove && MouseInGame)
        {
            var mouse = new Vector2((int)WorldMouse.X / 16 * 16, (int)WorldMouse.Y / 16 * 16);
            Player.DrawCursor(this, mouse, SpriteBatch);
        }
    }
}
