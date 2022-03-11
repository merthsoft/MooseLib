using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Monster;
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

    public Texture2D DungeonTiles = null!;
    public Texture2D MiniMapTiles = null!;
    public Texture2D CrosshairTexture = null!;
    public Vector2 CrosshairOrigin;

    private Texture2D CursorTexture = null!;

    public Texture2D MapTexture = null!;
    public Texture2D MapCornerTexture = null!;

    SpriteFont DebugFont = null!;
    SpriteFont FallingTextFont = null!;


    public int BaseTileWidth = 16;
    public int BaseTileHeight = 16;

    public int DungeonSize = 34;

    public Window UxWindow = null!;

    public MapPanel MapPanel = null!;

    public int ViewingMap = 0;

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

    public DungeonMap DungeonMap = null!;

    DungeonPlayerDef PlayerDef = new();
    public DungeonPlayer Player = null!;

    List<FallingText> FallingTexts = new List<FallingText>();

    public bool CanPlay = true;

    public DungeonGame()
    {
        Instance = this;
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
        Window.Title = "Wiggle Wizzard";
        CrosshairTexture = ContentManager.LoadImage("Crosshair");
        CursorTexture = ContentManager.LoadImage("Cursor");
        MiniMapTiles = ContentManager.LoadImage("MiniMap");
        MapTexture = ContentManager.LoadImage("MapWindow");
        MapCornerTexture = ContentManager.LoadImage("MapCorner");

        CrosshairOrigin = new(CrosshairTexture.Width/2, CrosshairTexture.Height/2);

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

        Player.LearnSpell(AddSpellDef(new FireballDef(), (spellDef, owner, position) => new Fireball(spellDef, owner, position)));
        Player.LearnSpell(AddSpellDef(new MeteorDef(), (spellDef, owner, position)
            => 
            new SpellContainer(owner)
                .Add(new Meteor(spellDef, owner, position))
                .Add(new Meteor(spellDef, owner, position - new Vector2(16, 0)))
                .Add(new Meteor(spellDef, owner, position - new Vector2(-16, 0)))
                .Add(new Meteor(spellDef, owner, position - new Vector2(0, 16)))
                .Add(new Meteor(spellDef, owner, position - new Vector2(0, -16)))
            ));
        Player.LearnSpell(AddSpellDef(new SpinesDef(), (spellDef, owner, position) => new Spines(spellDef, owner, position)));
        AddSpellDef(new SpellDef("Dark Shield", "Shield"), (spellDef, owner, position) => new Fireball(spellDef, owner, position));
        AddSpellDef(new SpellDef("AnimateDead", "Raise Dead"), (spellDef, owner, position) => new Fireball(spellDef, owner, position));
        AddSpellDef(new SpellDef("Slow", "Tanglefoot"), (spellDef, owner, position) => new Fireball(spellDef, owner, position));
        AddSpellDef(new SpellDef("Lightning"), (spellDef, owner, position) => new Fireball(spellDef, owner, position));

        AddMonsterDef(new MonsterDef("Marshall", MonsterTile.Marshall)
        {
            HitPoints = 5,
        }, (def, x, y) => new Marshall(def, new Vector2(x * 16, y * 16)));

        var fonts = new[] {
            ContentManager.BakeFont("Wizard's Manse", 180),
            ContentManager.BakeFont("Wizard's Manse", 50),
        };

        UxWindow = new(
            new(ContentManager.LoadImage("Window"), 32, 32,
                fonts, controlDrawOffset: new(16, 16))
            {
                TextColor = Color.DarkGray,
                TextMouseOverColor = Color.DeepPink,
                SelectedColor = Color.Gold,
                SelectedMouseOverColor = Color.HotPink,
            }, 0, 0, 320, 960) 
        { BackgroundDrawingMode = BackgroundDrawingMode.None };


        var panel = UxWindow.AddPanel(0, 0, UxWindow.Width, UxWindow.Height);
        var spellBook = panel.AddControlPassThrough(new SpellBookBar(UxWindow, 0, 0));
        MapPanel = panel.AddControlPassThrough(new MapPanel(miniMapRenderer, UxWindow, 0, spellBook.Height, 288, 288));
    }

    protected override void PostLoad()
    {
        AddObject(Player);
        
        DungeonMap.GenerateDungeon();
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
        
        if (x == playerX && y == playerY)
            return MiniMapTile.Player;
        
        if (DungeonMap.MonsterLayer.Objects.Any(m => m.GetCell() == mapCell))
            return MiniMapTile.Monster;
        
        return (MiniMapTile)DungeonMap.DungeonLayer.GetTileValue(x, y);
    }

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
        var spell = SpellBook.Cast(spellDef, owner, position);
        if (spell is SpellContainer container)
        {
            owner.RemoveSpell(spell);
            foreach (var childSpell in container)
                AddObject(childSpell);
        }
        else
            AddObject(spell);
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
        MainCamera.Position = Player.Position - ScreenSize / MainCamera.Zoom / 2f;
        MainCamera.Position = MainCamera.Position with
        {
            X = MainCamera.Position.X - 320f / 4f
        };

        if (WasKeyJustPressed(Keys.D))
        {
            DungeonMap.GenerateDungeon();
            Player.UseVisionCircle = true;
        }
        else if (WasKeyJustPressed(Keys.T))
        {
            DungeonMap.GenerateTown(5, roomSizes);
            Player.Position = new(16, 16);
            Player.UseVisionCircle = false;
        }
        else if (WasKeyJustPressed(Keys.Escape))
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
                Tweener.TweenTo(MainCamera, m => m.Zoom, 2, .35f);
            else if (MainCamera.Zoom == 2)
                Tweener.TweenTo(MainCamera, m => m.Zoom, 3, .35f);
        }

        CanPlay = ViewingMap == 0 && !ReadObjects.OfType<DungeonObject>().Any(o => o.CurrentlyBlockingInput);

        FallingTexts.RemoveAll(text => text.Done || text.Age++ > 1500);
        UxWindow.Update(gameTime);
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
        if (CanPlay && Player.CanMove)
        {
            var mouse = new Vector2((int)WorldMouse.X / 16 * 16, (int)WorldMouse.Y / 16 * 16);
            Player.DrawCursor(this, mouse, SpriteBatch);
        }
    }
}
