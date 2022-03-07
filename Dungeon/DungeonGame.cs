using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

namespace Merthsoft.Moose.Dungeon;

public class DungeonGame : MooseGame
{
    internal static int BaseTileWidth = 16;
    internal static int BaseTileHeight = 16;
    
    internal static int DungeonSize = 40;

    internal static int DungeonNumberOfRooms = 10;
    internal static int TownNumberOfRooms = 5;

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
    DungeonPlayer Player { get; }

    SpriteFont DebugFont = null!;

    public DungeonGame()
    {
        Player = new(PlayerDef) { Position = new(DungeonSize / 2 * BaseTileWidth, DungeonSize / 2 * BaseTileHeight) };
    }

    internal static Tile GetDungeonTile(int x, int y)
        => DungeonMap.GetLayer<DungeonLayer>(0).GetTileValue(x, y);


    protected override void Load()
    {
        DefaultBackgroundColor = new(16, 16, 16);
        var tiles = ContentManager.LoadImage("Dungeon");

        var dungeonRenderer = new DungeonRenderer(SpriteBatch, BaseTileWidth, BaseTileHeight, tiles);
        dungeonRenderer[(int)Tile.StoneWall] = ContentManager.LoadImage("StoneWall");
        dungeonRenderer[(int)Tile.BrickWall] = ContentManager.LoadImage("BrickWall");
        AddDefaultRenderer<DungeonLayer>("dungeon", dungeonRenderer);
        AddDefaultRenderer<ObjectLayer>("objects", new SpriteBatchObjectRenderer(SpriteBatch));

        DungeonMap = new DungeonMap(DungeonSize, DungeonSize);
        GenerateDungeon();
        ActiveMaps.Add(DungeonMap);

        SetScreenSize(1280, 960);
        ZoomIn(1);
        AddDef(PlayerDef);
        DebugFont = ContentManager.BakeFont("MatchupPro", 30);
    }

    private void GenerateDungeon()
        => DungeonMap.GenerateDungeon(DungeonNumberOfRooms, roomSizes);

    private void GenerateTown()
        => DungeonMap.GenerateTown(TownNumberOfRooms, roomSizes.Select(a => (a.w + 2, a.h + 2)).ToArray());

    protected override void PreUpdate(GameTime gameTime)
    {
        MainCamera.Position = Player.Position - ScreenSize / MainCamera.Zoom / 2;

        if (WasKeyJustPressed(Keys.D))
            GenerateDungeon();
        else if (WasKeyJustPressed(Keys.T))
            GenerateTown();
            
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
