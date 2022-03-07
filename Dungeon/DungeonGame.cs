using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

namespace Merthsoft.Moose.Dungeon;

public class DungeonGame : MooseGame
{
    
    internal int BaseTileWidth = 16;
    internal int BaseTileHeight = 16;
    
    internal int DungeonSize = 45;

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

    internal static DungeonMap DungeonMap { get; set; } = null!;

    DungeonPlayerDef PlayerDef { get; } = new();
    internal static DungeonPlayer Player { get; private set; } = null!;

    SpriteFont DebugFont = null!;

    public DungeonGame()
    {
        Player = new(PlayerDef) { };
    }

    internal Tile GetDungeonTile(int x, int y)
        => DungeonMap.GetLayer<DungeonLayer>(0).GetTileValue(x, y);


    protected override void Load()
    {
        DefaultBackgroundColor = new(16, 16, 16);
        var tiles = ContentManager.LoadImage("Dungeon");

        var dungeonRenderer = new DungeonRenderer(Player, SpriteBatch, BaseTileWidth, BaseTileHeight, tiles);
        dungeonRenderer[(int)Tile.StoneWall] = ContentManager.LoadImage("StoneWall");
        dungeonRenderer[(int)Tile.BrickWall] = ContentManager.LoadImage("BrickWall");
        AddDefaultRenderer<DungeonLayer>("dungeon", dungeonRenderer);
        AddDefaultRenderer<ObjectLayer>("objects", new SpriteBatchObjectRenderer(SpriteBatch));

        DungeonMap = new DungeonMap(DungeonSize, DungeonSize);
        DungeonMap.GenerateRandomLevel();
        ActiveMaps.Add(DungeonMap);

        SetScreenSize(1280, 960);
        ZoomIn(2);
        AddDef(PlayerDef);
        DebugFont = ContentManager.BakeFont("MatchupPro", 30);
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
