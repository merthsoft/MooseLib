using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

namespace Merthsoft.Moose.Dungeon;

public class DungeonGame : MooseGame
{
    public static int BaseTileWidth = 16;
    public static int BaseTileHeight = 16;

    internal static DungeonMap DungeonMap { get; set; }

    DungeonPlayerDef PlayerDef { get; } = new();
    DungeonPlayer Player { get; }

    SpriteFont DebugFont = null!;

    string map = @"
XXXXXXXX
X......X
X......X
X......X
X......],,,,,,,,
XXXXXXXX       ,
               ,
               ,
           YYYY=YYY
           Y______Y
           Y______Y
           YC_____Y
           YYYYYYYY
";

    public DungeonGame()
    {
        Player = new(PlayerDef) { Position = new(16, 16) };
    }

    internal static Tile GetDungeonTile(int x, int y)
        => DungeonMap.GetLayer<DungeonLayer>(0).GetTileValue(x, y);


    protected override void Load()
    {
        DefaultBackgroundColor = new(16, 16, 16);
        var tiles = ContentManager.LoadImage("Dungeon");

        var dungeonRenderer = new SpriteBatchAutoTileTextureRenderer(SpriteBatch, BaseTileWidth, BaseTileHeight, tiles);
        dungeonRenderer.AutoTileTextureMap[(int)Tile.StoneWall] = ContentManager.LoadImage("StoneWall");
        dungeonRenderer.AutoTileTextureMap[(int)Tile.BrickWall] = ContentManager.LoadImage("BrickWall");
        AddDefaultRenderer<DungeonLayer>("dungeon", dungeonRenderer);
        AddDefaultRenderer<ObjectLayer>("objects", new SpriteBatchObjectRenderer(SpriteBatch));

        DungeonMap = new DungeonMap();
        ActiveMaps.Add(DungeonMap);

        var x = 0;
        var y = 0;
        var lines = map.Split('\n');
        foreach (var line in lines.Skip(1))
        {
            foreach (var c in line.TrimEnd())
            {
                var tile = c switch
                {
                    'X' => Tile.StoneWall,
                    ']' => Tile.GreenDoorVertical,
                    '.' => Tile.GrayLargeCheckersFloor,
                    '_' => Tile.RedDotsFloor,
                    ',' => Tile.GrayCobblestoneFloor,
                    'Y' => Tile.BrickWall,
                    'C' => Tile.ClosedChest,
                    '=' => Tile.RedDoorHorizontal,
                    _ => Tile.None,
                };
                DungeonMap.GetLayer<DungeonLayer>(0).SetTileValue(x, y, tile);
                x++;
            }
            x = 0;
            y++;
        }

        ZoomIn(3);
        SetScreenSize(1280, 960);

        AddDef(PlayerDef);

        DebugFont = ContentManager.BakeFont("MatchupPro", 12);
    }

    protected override void PostLoad()
    {
        AddObject(Player);
    }
}
