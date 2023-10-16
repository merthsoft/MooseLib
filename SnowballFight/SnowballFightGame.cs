using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.Tiled;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tweening;
using System.Diagnostics;
using System.Reflection;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;

namespace Merthsoft.Moose.SnowballFight;

public class SnowballFightGame : MooseGame
{
    public const int WindowSize = 960;

    private enum GameMode { Demo, SelectingTeam, Playing, Paused };
    private GameMode Mode { get; set; } = GameMode.Demo;
    private Team? PlayerTeam = null;

    public new static SnowballFightGame Instance { get; private set; } = null!;
    public new TiledMooseMap MainMap { get; private set; }

    public static string UnitLayer { get; private set; } = "Units";
    public static string SnowballLayer { get; private set; } = "Snowballs";

    private Window UxWindow = null!;
    private Panel StatsWindow = null!;
    private MainMenu MainMenu = null!;
    private PauseMenu PauseMenu = null!;
    private TeamSelectionWindow TeamSelectionWindow = null!;
    private SpriteFont DebugFont = null!;

    public OrthographicCamera StatsWindowCamera = null!;

    private readonly Dictionary<Vector2, Color> SelectedUnitHintCells = new();

    private Unit? SelectedUnit { get; set; }

    private Unit? TargettedUnit { get; set; }

    private Texture2D UnitsTexture = null!;

    private IEnumerable<Unit> Units => ReadObjects.OfType<Unit>();

    public static AnimatedGameObjectDef SnowballDef => GetDef<AnimatedGameObjectDef>("snowball")!;

    private readonly Dictionary<string, RenderHook> GameRenderHooks;
    public override Dictionary<string, RenderHook>? LayerRenderHooks => Mode == GameMode.Playing ? GameRenderHooks : null;

    public readonly string VersionString;

    public SnowballFightGame()
    {
        Instance = this;

        GameRenderHooks = new()
        {
            { "Lower Decoration", new(PostHook: _ => DrawSelectedUnitDetails()) },
            { "Snowballs", new(PostHook: _ => DrawTargetLine()) },
        };

        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        var version = fileVersionInfo.ProductVersion!.Split('-');

        VersionString = $"v{version[0]}{version[1][0]} - {fileVersionInfo.LegalCopyright}";
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        ScreenHeight = WindowSize,
        ScreenWidth = WindowSize,
        IsMouseVisible = true
    };

    protected override void Load()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        Window.Title += $"Snowfight - v{fileVersionInfo.ProductVersion}";

        AddLayerRenderer("map", new TiledMooseMapRenderer(GraphicsDevice));
        AddLayerRenderer("object", new SpriteBatchObjectRenderer(SpriteBatch));

        LoadMap("title_screen");

        UnitsTexture = Content.Load<Texture2D>("Images/units");
        var unitsTextureData = new Color[UnitsTexture.Width * UnitsTexture.Height];
        UnitsTexture.GetData(unitsTextureData);

        Texture2D extractPortrait(int portraitNo)
        {
            var t = new Texture2D(GraphicsDevice, 16, 16);
            var textureData = new Color[16 * 16];
            for (var y = 0; y < 16; y++)
                for (var x = 0; x < 16; x++)
                    textureData[x + y * 16] = unitsTextureData![x + y * UnitsTexture.Width + portraitNo * UnitsTexture.Width * 16];
            t.SetData(textureData);
            return t;
        }

        var krampusPortrait = extractPortrait(8);
        var santaPortrait = extractPortrait(9);

        AddDef(new UnitDef("deer", 6, 4, 6, 1, extractPortrait(10)));
        AddDef(new UnitDef("elf1", 4, 6, 5, .2f, extractPortrait(3)));
        AddDef(new UnitDef("elf2", 4, 6, 5, .2f, extractPortrait(4)));
        AddDef(new UnitDef("elf3", 4, 6, 5, .2f, extractPortrait(5)));
        AddDef(new UnitDef("krampus", 8, 8, 4, .1f, krampusPortrait));
        AddDef(new UnitDef("mari", 3, 8, 11, .4f, extractPortrait(1), "Mari Lwyd"));
        AddDef(new UnitDef("santa", 8, 8, 4, .1f, santaPortrait));
        AddDef(new UnitDef("snowman", 8, 2, 4, .1f, extractPortrait(2)));

        AddDef(new AnimatedGameObjectDef("snowball", "snowball"));
        AddDef(new AnimatedGameObjectDef("snow", "snow"));

        MainCamera.ZoomIn(1f);

        var mainMenuFonts = new SpriteFont[]
        {
            ContentManager.BakeFont("Formal_Future", 64),
            ContentManager.BakeFont("Direct_Message", 48),
            ContentManager.BakeFont("Tomorrow_Night", 16),
            ContentManager.BakeFont("Direct_Message", 40),
            ContentManager.BakeFont("Royal_Decree", 20),
            ContentManager.BakeFont("Formal_Future", 84),
            ContentManager.BakeFont("Simply_Social", 40),
        };

        DebugFont = ContentManager.BakeFont("Outward_Bound_Monospaced", 16);

        var gameFonts = new SpriteFont[]
        {
            ContentManager.BakeFont("Whacky_Joe", 32),
            ContentManager.BakeFont("Direct_Message", 24),
        };

        var windowTextures = new Texture2D[]
        {
            Content.Load<Texture2D>("Images/window1"),
            Content.Load<Texture2D>("Images/window2"),
        };

        var themes = new Theme[] {
            new(windowTextures[0], 32, 32, mainMenuFonts) { TextureWindowControlDrawOffset = new(8, 6), TextColor = Color.White, TextMouseOverColor = Color.Yellow, TextBorderColor = Color.Black },
            new(windowTextures[0], 32, 32, gameFonts) { TextureWindowControlDrawOffset = new(6, 6), TextColor = Color.White, TextMouseOverColor = Color.Yellow, TextBorderColor = Color.Black },
        };

        UxWindow = new Window(themes[0], 0, 0, WindowSize, WindowSize) { BackgroundDrawingMode = BackgroundDrawingMode.None };
        UxWindow.OverlayColor = Color.Black;
        var windowTween = UxWindow.TweenToOverlayAlpha(0f, 1f);

        PauseMenu = new PauseMenu(MainMenu_Clicked, UxWindow);
        PauseMenu.Center(WindowSize, WindowSize);
        PauseMenu.Position -= new Vector2(0, PauseMenu.Y + PauseMenu.Height);
        UxWindow.AddControl(PauseMenu);

        MainMenu = new MainMenu(MainMenu_Clicked, UxWindow);
        MainMenu.Center(WindowSize, WindowSize);
        MainMenu.Position -= new Vector2(0, 900);
        windowTween.OnEnd(_ => 
            MainMenu.TweenToCenter(WindowSize, WindowSize, 1.7f, 
                easingFunction: EasingFunctions.BounceOut));
        UxWindow.AddControl(MainMenu);

        TeamSelectionWindow = new TeamSelectionWindow(TeamSelectionWindow_TeamSelected, UxWindow, santaPortrait, krampusPortrait);
        TeamSelectionWindow.Center(WindowSize, WindowSize);
        TeamSelectionWindow.Position += new Vector2(0, WindowSize);
        UxWindow.AddControl(TeamSelectionWindow);

        StatsWindow = UxWindow.AddPanel(0, 416 * 2, 480 * 2, 64 * 2);
        StatsWindow.Position -= new Vector2(WindowSize*2, 0);
        StatsWindow.Hide();

        ContentManager.LoadAnimatedSpriteSheet(Snowball.AnimationKey);
    }

    protected override void PostLoad()
        => StartDemo();

    private void MainMenu_Clicked(string option)
    {
        switch (option)
        {
            case "New Game":
                NewGame();
                break;
            case "Resume":
                PauseMenu.TweenToOffset(new(0, -PauseMenu.Height), .5f,
                        onEnd: _ => Mode = GameMode.Playing);
                break;
            case "Quit":
                PauseMenu.TweenToOffset(new(0, -PauseMenu.Height), .5f,
                    onEnd: _ => StartDemo());
                UxWindow.FadeToColor(.5f, 
                    onEnd: _ => MainMenu.TweenToCenter(ScreenWidth, ScreenHeight, .5f), 
                    autoReverse: true);
                break;
            case "Exit":
                ShouldQuit = true;
                break;
        }
    }

    private void TeamSelectionWindow_TeamSelected(Team? team)
    {
        var tween = TeamSelectionWindow.TweenToOffset(new(0, WindowSize), .5f);
        if (team == null)
        {
            MainMenu.TweenToCenter(ScreenWidth, ScreenHeight, .5f);
            tween.OnEnd(_ => Mode = GameMode.Demo);
            return;
        }
        tween.OnEnd(_ => StartGame());
        UxWindow.FadeToColor(.5f, autoReverse: true);
        PlayerTeam = team.Value;
    }

    private Unit SpawnUnit(string unitDef, int cellX, int cellY, string state = Unit.States.Idle)
    {
        var unit = new Unit(GetDef<UnitDef>(unitDef), cellX * TileWidth, cellY * TileHeight, state);
        AddObject(unit);
        return unit;
    }

    private void NewGame()
    {
        Mode = GameMode.SelectingTeam;
        MainMenu.TweenToPosition(new(MainMenu.X, -MainMenu.Height), .5f);
        TeamSelectionWindow.TweenToCenter(ScreenWidth, ScreenHeight, .5f);
    }

    private void StartDemo()
    {
        LoadMap("title_screen");
        Mode = GameMode.Demo;

        for (var index = 0; index < 8; index++)
            SpawnUnit(Random.Next(0, 3) switch
            {
                1 => "elf1",
                2 => "elf2",
                _ => "elf3"
            }, Random.Next(4, 26), Random.Next(21, 29));
    }

    private void StartGame()
    {
        Mode = GameMode.Playing;
        LoadMap("map1");

        MarkAllObjectsForRemoval();
        SelectedUnit = null;

        SpawnUnit("deer", 5, 9);
        SpawnUnit("elf1", 10, 5);
        SpawnUnit("elf2", 3, 10);
        SpawnUnit("elf3", 11, 11);

        SpawnUnit("krampus", 12, 14);
        SpawnUnit("mari", 13, 14);

        SpawnUnit("santa", 13, 12);
        SpawnUnit("snowman", 11, 13);
    }

    private void LoadMap(string mapName)
    {
        ActiveMaps.Clear();
        MainMap = new TiledMooseMap(Content.Load<TiledMap>($"Maps/{mapName}"));
        ActiveMaps.Add(MainMap);
        GetRenderer<TiledMooseMapRenderer>("map").Load(MainMap!);
    }

    public static Snowball? SpawnSnowball(IEnumerable<Vector2> flightPath)
    {
        if (!flightPath.Any())
            return null;
        return Instance.AddObject(new Snowball(flightPath));
    }

    protected override void PreUpdate(GameTime gameTime)
        => HandleInput();

    protected override void PostObjectsUpdate(GameTime gameTime)
    {
        UxWindow.Update(gameTime);
        if (Mode == GameMode.Demo || Mode == GameMode.SelectingTeam)
            DemoUpdate();
    }

    private void DemoUpdate()
    {
        if (SelectedUnit == null && Units.Any())
        {
            SelectedUnit = Units.ElementAt(Random.Next(Units.Count()));
            var state = Random.Next(2) switch
            {
                0 => Unit.States.Walk,
                _ => Unit.States.Attack
            };

            SelectedUnit.State = state;

            if (state == Unit.States.Walk)
            {
                var startCell = SelectedUnit.Cell;
                var endCell = new Point(startCell.X + Random.Next(-3, 4), startCell.Y + Random.Next(-3, 4));
                var path = MainMap.FindCellPath(startCell, endCell);
                if (path.Any())
                    path.ForEach(SelectedUnit.MoveQueue.Enqueue);
            }
            else if (state == Unit.States.Attack)
            {
                var attackUnit = (ReadObjects[Random.Next(6)] as Unit)!;
                while (attackUnit == SelectedUnit)
                    attackUnit = (ReadObjects[Random.Next(6)] as Unit)!;

                SelectedUnit.Attack(attackUnit);
            }
        }

        if (SelectedUnit?.State == Unit.States.Idle)
            SelectedUnit = null;
    }

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        UxWindow.Draw(SpriteBatch, gameTime);

        if (Mode == GameMode.Demo)
        {
            var stringHeight = DebugFont.MeasureString(VersionString).Y;
            SpriteBatch.DrawString(DebugFont, VersionString, new(0, ScreenHeight - stringHeight), Color.Black);
        }

        SpriteBatch.End();
    }

    private void HandleInput()
    {
        if (Mode == GameMode.Demo || Mode == GameMode.SelectingTeam)
            return;

        if (WasKeyJustPressed(Keys.Escape))
        {
            switch (Mode)
            {
                case GameMode.Paused:
                    PauseMenu.TweenToOffset(new(0, -PauseMenu.Height), .5f,
                        onEnd: _ => Mode = GameMode.Playing);
                    break;
                case GameMode.Playing:
                    PauseMenu.TweenToOffset(new(0, PauseMenu.Height), .5f,
                        onEnd: _ => Mode = GameMode.Paused);
                    break;
            }

            return;
        }

        if (Mode == GameMode.Paused)
            return;

        var mouseOverUnit = Units.FirstOrDefault(unit => unit.AtWorldPosition(WorldMouse));

        if (CurrentMouseState.LeftButton.JustPressed(PreviousMouseState.LeftButton))
            HandleLeftClick();
        else if (CurrentMouseState.RightButton.JustPressed(PreviousMouseState.RightButton))
            HandleRightClick();
        else if (SelectedUnit != mouseOverUnit)
            TargettedUnit = mouseOverUnit;

        void HandleRightClick()
            => ClearSelectUnits();

        void HandleLeftClick()
        {
            if (SelectedUnit == null)
            {
                if (mouseOverUnit != null && mouseOverUnit.State == Unit.States.Idle)
                    SelectSingleUnit(mouseOverUnit);
            }
            else if (TargettedUnit == null)
            {
                var unitCell = SelectedUnit.Cell;
                var mouseCell = MainCamera.ScreenToWorld(
                            CurrentMouseState.Position.X / TileWidth * TileWidth,
                            CurrentMouseState.Position.Y / TileHeight * TileHeight);
                mouseCell /= TileSize;

                var path = MainMap.FindCellPath(unitCell, mouseCell.ToPoint());
                if (path.Any() && path.Count() <= SelectedUnit.DisplaySpeed)
                {
                    path.ForEach(SelectedUnit.MoveQueue.Enqueue);
                    SelectedUnit.State = Unit.States.Walk;
                    ClearSelectUnits();
                }
            }
            else
            {
                SelectedUnit.Attack(TargettedUnit);
                ClearSelectUnits();
            }
        }
    }

    private void SelectSingleUnit(Unit unit)
    {
        ClearSelectUnits();
        StatsWindow.Show();
        SelectedUnit = unit;

        var displayNameSize = StatsWindow.Theme.MeasureString(unit.DisplayName, 0);
        var smallFontHeight = (int)StatsWindow.Theme.MeasureString("|", 1).Y;

        StatsWindow.AddPicture(4, 4, unit.Portrait, 6);
        var nameLabel = StatsWindow.AddLabel(113, 2, unit.DisplayName, color: Color.Yellow);
        var yOffset = (int)displayNameSize.Y + 2;
        StatsWindow.AddLine(112, yOffset, 112 + (int)displayNameSize.X, yOffset);
        StatsWindow.AddLabel(112, yOffset + 2, $"Speed: {unit.DisplaySpeed}", 1);
        StatsWindow.AddLabel(112, yOffset + 2 + smallFontHeight * 1, $"HP: {unit.DisplayHealth}/{unit.DisplayMaxHealth}", 1);
        StatsWindow.AddLabel(112, yOffset + 2 + smallFontHeight * 2, $"Accuracy: {unit.DisplayAccuracy}", 1);

        var unitCell = unit.Cell;
        var cachedGrid = MainMap.BuildCollisionGrid(unitCell);
        for (var x = 0; x < MapWidth; x++)
            for (var y = 0; y < MapHeight; y++)
            {
                var deltaCell = new Vector2(x, y);

                if (SelectedUnitHintCells.ContainsKey(deltaCell * TileSize))
                    continue;

                if (MainMap.GetBlockingVector(x, y).Any(b => b > 0))
                    continue;

                var path = MainMap.FindCellPath(unitCell, deltaCell.ToPoint(), cachedGrid);

                if (!path.Any())
                    continue;

                var pathCount = 0;
                foreach (var p in path)
                {
                    pathCount++;
                    var color = pathCount <= unit.DisplaySpeed
                        ? pathCount - 1 <= unit.DisplaySpeed / 2
                            ? Color.Green.HalveAlphaChannel()
                            : Color.DarkOrange.HalveAlphaChannel()
                        : Color.Transparent;

                    SelectedUnitHintCells[p.ToVector2() * TileSize] = color;
                }
            }
    }

    private void ClearSelectUnits()
    {
        SelectedUnit = null;
        SelectedUnitHintCells.Clear();
        TargettedUnit = null;
        StatsWindow.ClearControls();
        StatsWindow.Hide();
    }

    private void DrawTargetLine()
    {
        if (TargettedUnit == null || SelectedUnit == null)
            return;

        var selectedUnitCell = SelectedUnit.Cell;
        var targettedUnitCell = TargettedUnit.Cell;

        var startWorldPosition = SelectedUnit.Position + HalfTileSize;
        var endWorldPosition = TargettedUnit.Position + HalfTileSize;
        var leftCone = endWorldPosition.RotateAround(startWorldPosition, 5).GetFloor();
        var rightCone = endWorldPosition.RotateAround(startWorldPosition, -5).GetFloor();

        void drawLineTo(Vector2 start, Vector2 end, Color color, bool extend, int thickness)
        {
            foreach (var worldPosition in start.CastRay(end, true, true, extend: extend))
            {
                SpriteBatch.DrawPoint(worldPosition, color, thickness);
                var cell = (worldPosition / TileSize).GetFloor().ToPoint();
                var blockedVector = MainMap.GetBlockingVector((int)worldPosition.X, (int)worldPosition.Y);
                if (blockedVector.Skip(2).Any(b => b > 0) && cell != selectedUnitCell && cell != targettedUnitCell)
                    break;
            }
        }

        drawLineTo(startWorldPosition, endWorldPosition, Color.Green, false, 3);
        drawLineTo(startWorldPosition, leftCone, Color.Red, true, 1);
        drawLineTo(startWorldPosition, rightCone, Color.Red, true, 1);
    }

    private void DrawSelectedUnitDetails()
    {
        if (SelectedUnit == null)
            return;

        var transformMatrix = MainCamera.GetViewMatrix();
        SpriteBatch.Begin(transformMatrix: transformMatrix);

        SpriteBatch.FillRectangle(SelectedUnit.Position, TileSize, Color.Red.HalveAlphaChannel());

        SelectedUnitHintCells.ForEach(t =>
        {
            SpriteBatch.DrawRectangle(new(t.Key.X, t.Key.Y, TileWidth + 1, TileHeight + 1), Color.LightGray);
            SpriteBatch.FillRectangle(new(t.Key.X + 1, t.Key.Y + 1, TileWidth - 1, TileHeight - 1), t.Value);
        });

        var mouseCell = MainCamera.ScreenToWorld(
                            CurrentMouseState.Position.X / TileWidth * TileWidth,
                            CurrentMouseState.Position.Y / TileHeight * TileHeight);
        mouseCell /= TileSize;

        var unitCell = SelectedUnit.Cell;
        var mousePath = MainMap.FindCellPath(unitCell, mouseCell.ToPoint());
        var mousePathCount = mousePath.Count();
        if (mousePathCount > 0 && mousePathCount <= SelectedUnit.DisplaySpeed)
        {
            var lastCell = unitCell.ToVector2() * TileSize + HalfTileSize;
            var index = 0;
            foreach (var p in mousePath)
            {
                var nextCell = new Vector2(p.X * TileWidth, p.Y * TileHeight) + HalfTileSize;
                SpriteBatch.DrawLine(lastCell, nextCell, Color.Black, 2);
                SpriteBatch.DrawCircle(lastCell, 2, 40, Color.Red, 2);
                lastCell = nextCell;
                index++;
            }
            SpriteBatch.DrawCircle(lastCell, 2, 40, Color.Red, 2);
        }

        SpriteBatch.End();
    }
}
