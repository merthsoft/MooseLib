namespace Merthsoft.Moose.Island;

public enum IslandTile { Water = 1, Land }

public class IslandGame : MooseGame
{
    public static Texture2D TerrainImage = null!;
    public static IslandMap Map = null!;
    public static SpriteBatchWangTileTextureRenderer<IslandTile> Renderer = null!;

    public IslandGame()
    {
     
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        ScreenWidth = 1000,
        ScreenHeight = 1000,
    };

    protected override void Load()
    {
        TerrainImage = ContentManager.LoadImage("Terrain");

        Renderer = new SpriteBatchWangTileTextureRenderer<IslandTile>(SpriteBatch, 8, 8, TerrainImage);
        InitializeRenderer();
        ActiveMaps.Add(Map = new IslandMap());
        AddDefaultRenderer<TileLayer<IslandTile>>("island", Renderer);

        MainCamera.ZoomIn(0);
    }

    private void InitializeRenderer()
    {
        Renderer.WangDefinitions[IslandTile.Land] = 1;
        Renderer.WangDefinitions[IslandTile.Water] = 2;

        Renderer.WangTiles = new()
        {
            new(3, "2,2,2,1,2,2,2,2")  { AppliesTo = 2 },
            new(4, "2,2,2,0,1,0,2,2")  { AppliesTo = 2 },
            new(5, "2,2,2,2,2,1,2,2")  { AppliesTo = 2 },
            new(6, "1,1,1,1,1,0,2,0") { AppliesTo = 2 },
            new(7, "1,0,2,0,1,1,1,1") { AppliesTo = 2},
            new(24, "1,1,1,0,2,0,1,1") { AppliesTo = 2},
            new(23,"2,0,1,1,1,1,1,0") { AppliesTo = 2},
            new(19, "1,1,1,1,1,1,1,1") { AppliesTo = 2 },
            new(20, "2,0,1,0,2,2,2,2") { AppliesTo = 2 },
            new(22, "2,2,2,2,2,0,1,0") { AppliesTo = 2 },
            new(37, "2,0,1,0,2,2,2,2") { AppliesTo = 2 },
            new(39, "2,2,2,2,2,0,1,0") { AppliesTo = 2 },
            new(51, "2,2,2,1,2,2,2,2") { AppliesTo = 2 },
            new(52, "2,2,2,0,1,0,2,2") { AppliesTo = 2 },
            new(53, "2,2,2,0,1,0,2,2") { AppliesTo = 2 },
            new(54, "2,0,1,1,1,0,2,2") { AppliesTo = 2 },
            new(56, "2,2,2,0,1,1,1,0") { AppliesTo = 2 },
            new(57, "2,2,2,0,1,0,2,2") { AppliesTo = 2 },
            new(58, "2,2,2,0,1,0,2,2") { AppliesTo = 2 },
            new(59, "2,2,2,2,2,1,2,2") { AppliesTo = 2 },
            new(68, "2,0,1,0,2,2,2,2") { AppliesTo = 2 },
            new(76, "2,2,2,2,2,0,1,0") { AppliesTo = 2 },
            new(85, "2,1,2,2,2,2,2,2") { AppliesTo = 2 },
            new(86, "1,0,2,2,2,2,2,0") { AppliesTo = 2 },
            new(87, "1,0,2,2,2,2,2,0") { AppliesTo = 2 },
            new(88, "1,1,1,0,2,2,2,0") { AppliesTo = 2 },
            new(90, "1,0,2,2,2,0,1,1") { AppliesTo = 2 },
            new(91, "1,0,2,2,2,2,2,0") { AppliesTo = 2 },
            new(92, "1,0,2,2,2,2,2,0") { AppliesTo = 2 },
            new(93, "2,2,2,2,2,2,2,1") { AppliesTo = 2 },
            new(105, "2,0,1,0,2,2,2,2") { AppliesTo = 2 },
            new(107, "2,2,2,2,2,0,1,0") { AppliesTo = 2 },
            new(122, "2,0,1,0,2,2,2,2") { AppliesTo = 2 },
            new(124, "2,2,2,2,2,0,1,0") { AppliesTo = 2 },
            new(139, "2,1,2,2,2,2,2,2") { AppliesTo = 2 },
            new(140, "1,0,2,2,2,2,2,0") { AppliesTo = 2 },
            new(141, "2,2,2,2,2,2,2,1") { AppliesTo = 2 },
            new(170, "0,1,2,1,2,1,0,0") { AppliesTo = 2 },
            new(171, "1,0,2,0,1,0,2,0") { AppliesTo = 2 },
            new(172, "0,0,0,1,2,1,2,1") { AppliesTo = 2 },
            new(174, "1,0,2,0,1,0,2,0") { AppliesTo = 2 },
            new(176, "2,0,1,0,2,0,1,0") { AppliesTo = 2 },
            new(179, "2,2,2,1,2,1,2,2") { AppliesTo = 2 },
            new(187, "2,0,1,0,2,0,1,0") { AppliesTo = 2 },
            new(189, "2,0,1,0,2,0,1,0") { AppliesTo = 2 },
            new(191, "1,0,2,0,1,0,2,0") { AppliesTo = 2 },
            new(193, "2,0,1,0,2,0,1,0") { AppliesTo = 2 },
            new(195, "2,1,2,1,2,2,2,2") { AppliesTo = 2 },
            new(197, "2,2,2,2,2,1,2,1") { AppliesTo = 2 },
            new(204, "2,1,2,1,0,0,0,1") { AppliesTo = 2 },
            new(205, "1,0,2,0,1,0,2,0") { AppliesTo = 2 },
            new(206, "2,1,0,0,0,1,2,1") { AppliesTo = 2 },
            new(208, "1,0,2,0,1,0,2,0") { AppliesTo = 2 },
            new(210, "2,0,1,0,2,0,1,0") { AppliesTo = 2 },
            new(213, "2,1,2,2,2,2,2,1") { AppliesTo = 2 },

        };
        Renderer.RebuildCache();
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        base.PreUpdate(gameTime);

        if (IsLeftMouseDown())
            Map.Island.SetTileValue((int)(WorldMouse.X / 8), (int)(WorldMouse.Y / 8), IslandTile.Land, IsKeyDown(Keys.LeftControl) ? 3 : 1);
        else if (IsRightMouseDown())
            Map.Island.SetTileValue((int)(WorldMouse.X / 8), (int)(WorldMouse.Y / 8), IslandTile.Water, IsKeyDown(Keys.LeftControl) ? 1 : 0);
    
        if (WasKeyJustPressed(Keys.Tab))
        {
            if (Renderer.WangDefinitions.Count == 0)
            {
                InitializeRenderer();
            }
            else
            {
                Renderer.WangDefinitions.Clear();
                Renderer.WangTiles.Clear();
                Renderer.RebuildCache();
            }
        } else if (WasKeyJustPressed(Keys.R))
        { Map.RandomizeMap(); }
    }
}
