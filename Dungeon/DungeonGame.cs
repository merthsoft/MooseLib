using Merthsoft.Moose.Dungeon.Spells;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon;

public class DungeonGame : MooseGame
{
    public SpellBook SpellBook { get; } = new();
    public Texture2D Crosshair { get; set; } = null!;
    public Vector2 CrosshairOrigin { get; set; }

    public int BaseTileWidth = 16;
    public int BaseTileHeight = 16;
    
    public int DungeonSize = 45;

    public Window UxWindow = null!;

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

    public static DungeonMap DungeonMap { get; set; } = null!;

    DungeonPlayerDef PlayerDef { get; } = new();
    public static DungeonPlayer Player { get; private set; } = null!;

    SpriteFont DebugFont = null!;

    public DungeonGame()
    {
        Player = new(PlayerDef) { };
        IsMouseVisible = false;
    }

    public DungeonTile GetDungeonTile(int x, int y)
        => DungeonMap.DungeonLayer.GetTileValue(x, y);

    public MonsterTile GetMonsterTile(int x, int y)
    {
        var p = new Point(x, y);
        return MonsterTile.None;
    }

    public void Cast(SpellDef spellDef, GameObjectBase owner, Vector2 position)
    {
        AddObject(SpellBook.Cast(spellDef, owner, position));
    }

    protected override void Load()
    {
        DefaultBackgroundColor = new(25, 15, 20);
        Crosshair = ContentManager.LoadImage("Crosshair");
        CrosshairOrigin = new(Crosshair.Width/2, Crosshair.Height/2);

        var tiles = ContentManager.LoadImage("Dungeon");
        var dungeonRenderer = new DungeonRenderer(Player, SpriteBatch, BaseTileWidth, BaseTileHeight, tiles);
        
        dungeonRenderer[(int)DungeonTile.StoneWall] = ContentManager.LoadImage("StoneWall");
        dungeonRenderer[(int)DungeonTile.BrickWall] = ContentManager.LoadImage("BrickWall");
        AddDefaultRenderer<DungeonLayer>("dungeon", dungeonRenderer);
        AddDefaultRenderer<ObjectLayer>("objects", new SpriteBatchObjectRenderer(SpriteBatch));

        DungeonMap = new DungeonMap(DungeonSize, DungeonSize);
        DungeonMap.GenerateRandomLevel();
        ActiveMaps.Add(DungeonMap);

        SetScreenSize(1600, 900);
        ZoomIn(2);
        AddDef(PlayerDef);
        DebugFont = ContentManager.BakeFont("MatchupPro", 30);

        var fireball = new FireballDef();
        AddSpell(fireball, (def, owner, position) => new Fireball(def, owner, position));
        Player.KnownSpells.Add(fireball);

        //UxWindow.
    }

    private void AddSpell(SpellDef spellDef, Func<SpellDef, GameObjectBase, Vector2, Spell> generator)
    {
        AddDef(spellDef);
        SpellBook.AddSpell(spellDef, (spellDef, owner, position) => new Fireball(spellDef, owner, position));
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        MainCamera.Position = Player.Position - ScreenSize / MainCamera.Zoom / 2;

        if (WasKeyJustPressed(Keys.R))
        {
            Player.Position = Vector2.Zero;
            Player.UseVisionCone = true;
            DungeonMap.GenerateRandomLevel();
        }

        if (WasKeyJustPressed(Keys.T))
        {
            Player.Position = new(16, 16);
            Player.UseVisionCone = false;
            DungeonMap.GenerateTown(5, roomSizes);
        }

    }

    protected override void PostLoad()
    {
        AddObject(Player);
    }

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        SpriteBatch.DrawString(DebugFont,
            $"FPS: {FramesPerSecondCounter.FramesPerSecond}",
            new Vector2(1, 930), Color.Gray);
        
        SpriteBatch.End();
    }
}
