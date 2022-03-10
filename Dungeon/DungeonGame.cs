using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Monster;
using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.Dungeon.Ux;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.Dungeon;

public class DungeonGame : MooseGame
{
    public SpellBook SpellBook { get; } = new();
    public Dictionary<MonsterTile, (MonsterDef, Func<MonsterDef, int, int, DungeonMonster>)> MonsterFactory = new();

    public Texture2D Crosshair { get; set; } = null!;
    public Vector2 CrosshairOrigin { get; set; }

    public Texture2D MiniMapTiles { get; set; } = null!;

    public int BaseTileWidth = 16;
    public int BaseTileHeight = 16;

    public int DungeonSize = 45;

    public Window UxWindow = null!;
    public MapWindow MapWindow = null!;

    private readonly (int w, int h)[] roomSizes = new[]
        {
            (3 + 2, 3 + 2),
            (4 + 2, 4 + 2),
            (4 + 2, 3 + 2),
            (3 + 2, 4 + 2),
            (5 + 2, 5 + 2),
            (6 + 2, 6 + 2),
            (4 + 2, 6 + 2),
            (3 + 2, 7 + 2),
            (7 + 2, 6 + 2),
            (7 + 2, 4 + 2),
            (4 + 2, 7 + 2),
        };

    public DungeonMap DungeonMap { get; set; } = null!;

    DungeonPlayerDef PlayerDef { get; } = new();
    public DungeonPlayer Player { get; private set; } = null!;

    SpriteFont DebugFont = null!;
    SpriteFont FallingTextFont = null!;

    List<FallingText> FallingTexts = new List<FallingText>();

    public bool CanPlay { get; set; } = true;

    private Texture2D cursorTexture = null!;

    public DungeonGame()
    {
        Player = new(PlayerDef) { };
        IsMouseVisible = false;

        DefaultRenderHooks = new()
        {
            { "monsters", new(PostHook: _ => DrawCursor()) },
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
        Crosshair = ContentManager.LoadImage("Crosshair");
        CrosshairOrigin = new(Crosshair.Width/2, Crosshair.Height/2);

        cursorTexture = ContentManager.LoadImage("Cursor");

        MiniMapTiles = ContentManager.LoadImage("MiniMap");

        var tiles = ContentManager.LoadImage("Dungeon");
        var dungeonRenderer = new DungeonRenderer(Player, SpriteBatch, BaseTileWidth, BaseTileHeight, tiles); 
        
        dungeonRenderer[(int)DungeonTile.StoneWall] = ContentManager.LoadImage("StoneWall");
        dungeonRenderer[(int)DungeonTile.BrickWall] = ContentManager.LoadImage("BrickWall");
        AddDefaultRenderer<DungeonLayer>("dungeon", dungeonRenderer);
        AddDefaultRenderer<ObjectLayer>("objects", new DungeonObjectRenderer(SpriteBatch));

        DungeonMap = new DungeonMap(DungeonSize, DungeonSize);
        ActiveMaps.Add(DungeonMap);

        ZoomIn(2);
        AddDef(PlayerDef);
        DebugFont = ContentManager.BakeFont("MatchupPro", 30);
        FallingTextFont = ContentManager.BakeFont("Border_Basic_Monospaced", 24);

        Player.LearnSpell(AddSpellDef(new FireballDef(), (spellDef, owner, position) => new Fireball(spellDef, owner, position)));
        Player.LearnSpell(AddSpellDef(new SpellDef("Lightning"), (spellDef, owner, position) => new Fireball(spellDef, owner, position)));
        Player.LearnSpell(AddSpellDef(new SpellDef("Earth Shard", "Spines"), (spellDef, owner, position) => new Fireball(spellDef, owner, position)));
        Player.LearnSpell(AddSpellDef(new SpellDef("Dark Shield", "Shield"), (spellDef, owner, position) => new Fireball(spellDef, owner, position)));

        AddMonsterDef(new MonsterDef("Marshall", MonsterTile.Marshall)
        {
            HitPoints = 5,
        }, (def, x, y) => new Marshall(def, new Vector2(x * 16, y * 16)));

        var fonts = new[] {
            ContentManager.BakeFont("Wizard's Manse", 180),
            ContentManager.BakeFont("Wizard's Manse", 60),
        };

        UxWindow = new(
            new(ContentManager.LoadImage("Window"), 32, 32,
                fonts, controlDrawOffset: new(16, 16))
            {
                TextColor = Color.DarkGray,
                TextMouseOverColor = Color.DeepPink,
            }, 0, 0, 320, 960) 
        { BackgroundDrawingMode = BackgroundDrawingMode.Texture };

        MapWindow = new(this, 
            new(ContentManager.LoadImage("MapWindowSmall"), 16, 16,
                fonts, controlDrawOffset: new(8, 8))
            {
                TextColor = Color.DarkGray,
                TextMouseOverColor = Color.DeepPink,
            }, 13, 480);
    }

    protected override void PostLoad()
    {
        AddObject(Player);
        Player.Reset(this);
        DungeonMap.GenerateRandomLevel(this);
        UxWindow.AddControl(new SpellBookBar(Player, UxWindow, 0, 0));
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
        => (MiniMapTile)DungeonMap.DungeonLayer.GetTileValue(x, y);

    public DungeonTile GetDungeonTile(int x, int y)
        => DungeonMap.DungeonLayer.GetTileValue(x, y);

    public MonsterTile GetMonsterTile(int x, int y)
        => (DungeonMap.MonsterLayer.Objects.FirstOrDefault(o => o.InCell(x, y)) as DungeonMonster)?.MonsterDef?.Monster ?? MonsterTile.None;

    public DungeonCreature? GetMonster(int x, int y)
        => DungeonMap.MonsterLayer.Objects.FirstOrDefault(o => o.InCell(x, y)) as DungeonMonster;

    public DungeonMonster SpawnMonster(MonsterTile monsterTile, int x, int y)
    {
        var (def, generator) = MonsterFactory[monsterTile];
        return AddObject(generator(def, x, y));
    }

    public void Cast(SpellDef spellDef, DungeonObject owner, Vector2 position)
    {
        AddObject(SpellBook.Cast(spellDef, owner, position));
    }

    public void SpawnFallingText(FallingText text)
    {
        var x = Random.Next(-1, 2);
        var y = Random.Next(-1, 2);
        Tween(text, t => t.Position, new Vector2(1500 * x, 1500 * y), Random.NextSingle() + 1f,
            onEnd: _ => text.Done = true,
            easingFunction: EasingFunctions.ExponentialIn);
        FallingTexts.Add(text);
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        MainCamera.Position = Player.Position - ScreenSize / MainCamera.Zoom / 2;
        MainCamera.Position = MainCamera.Position with
        {
            X = MainCamera.Position.X - 320 / 4
        };

        if (WasKeyJustPressed(Keys.R))
        {
            Player.Reset(this);
            DungeonMap.GenerateRandomLevel(this);
        }

        if (WasKeyJustPressed(Keys.T))
        {
            Player.Reset(this);
            Player.Position = new(16, 16);
            Player.UseVisionCircle = false;
            DungeonMap.GenerateTown(5, roomSizes);
        }

        FallingTexts.RemoveAll(text => text.Done || text.Age++ > 1500);

        CanPlay = !ReadObjects.OfType<DungeonObject>().Any(o => o.CurrentlyBlockingInput);

        UxWindow.Update(gameTime);
        MapWindow.Update(gameTime);
    }

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        UxWindow.Draw(SpriteBatch);
        MapWindow.Draw(SpriteBatch);

        SpriteBatch.DrawString(DebugFont,
            $"FPS: {FramesPerSecondCounter.FramesPerSecond}",
            new(15, 920), DefaultBackgroundColor.Shade(3f));

        SpriteBatch.Draw(cursorTexture, CurrentMouseState.Position.ToVector2(), null, Color.White, 0, Vector2.Zero, 3, SpriteEffects.None, 1);
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
        if (CanPlay && Player.CanMove)
        {
            var mouse = new Vector2((int)WorldMouse.X / 16 * 16, (int)WorldMouse.Y / 16 * 16);
            Player.DrawCursor(this, mouse, SpriteBatch);
        }
    }
}
