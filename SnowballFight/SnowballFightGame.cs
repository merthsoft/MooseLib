using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.TiledDriver;
using Merthsoft.Moose.MooseEngine.Ui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.SnowballFight
{
    public class SnowballFightGame : MooseGame
    {
        public const int WindowSize = 960;

        private enum GameMode { Demo, Playing, Paused };
        private GameMode Mode { get; set; }

        public static SnowballFightGame Instance { get; private set; } = null!;

        // TODO: Move layer values into something else
        public static int UnitLayer { get; private set; } = 2;
        public static int SnowballLayer { get; private set; } = 4;
        public static int WeatherLayer { get; private set; } = 5;
        
        private WindowManager WindowManager = null!;
        private Window StatsWindow = null!;
        private SimpleMenu MainMenu = null!;

        public OrthographicCamera StatsWindowCamera = null!;

        private readonly Dictionary<Vector2, Color> SelectedUnitHintCells = new();

        private Unit? SelectedUnit { get; set; }

        private Unit? TargettedUnit { get; set; }

        private Texture2D UnitsTexture = null!;

        private IEnumerable<Unit> Units => ReadObjects.OfType<Unit>();

        private AnimatedGameObjectDef SnowballDef => (Defs["snowball"] as AnimatedGameObjectDef)!;

        private readonly Dictionary<int, RenderHook> GameRenderHooks;
        public override IDictionary<int, RenderHook>? DefaultRenderHooks => Mode == GameMode.Playing ? GameRenderHooks : null;

        public SnowballFightGame()
        {
            Instance = this;

            GameRenderHooks = new()
            {
                { 2, new(PreHook: _ => DrawSelectedUnitDetails()) },
                { 4, new(PostHook: _ => DrawTargetLine()) },
            };
        }

        protected override void Initialize()
        {
            base.Initialize();

            Graphics.PreferredBackBufferWidth = WindowSize;
            Graphics.PreferredBackBufferHeight = WindowSize;
            Graphics.ApplyChanges();
        }

        protected override void Load()
        {
            AddDefaultRenderer<TiledMooseTileLayer>("map", new TiledMooseMapRenderer(GraphicsDevice));
            AddDefaultRenderer<TiledMooseObjectLayer>("object", new SpriteBatchObjectRenderer(SpriteBatch));

            LoadMap("title_screen");

            var objectLayerCount = 0;
            for (var layerIndex = 0; layerIndex < MainMap.Layers.Count; layerIndex++)
            {
                var layer = MainMap.Layers[layerIndex];
                if (layer is not TiledMooseObjectLayer objectLayer)
                    continue;
                
                objectLayerCount++;

                switch (objectLayerCount)
                {
                    case 0:
                        UnitLayer = layerIndex;
                        break;
                    case 1:
                        SnowballLayer = layerIndex;
                        break;
                    case 2:
                        WeatherLayer = layerIndex;
                        break;
                }

                if (objectLayerCount == 3)
                    break;
            }
            
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

            AddDef(new UnitDef("deer", 6, 4, 6, 1, extractPortrait(10)));
            AddDef(new UnitDef("elf1", 4, 6, 5, .2f, extractPortrait(3)));
            AddDef(new UnitDef("elf2", 4, 6, 5, .2f, extractPortrait(4)));
            AddDef(new UnitDef("elf3", 4, 6, 5, .2f, extractPortrait(5)));
            AddDef(new UnitDef("krampus", 8, 8, 4, .1f, extractPortrait(8)));
            AddDef(new UnitDef("mari", 3, 8, 11, .4f, extractPortrait(1), "Mari Lwyd"));
            AddDef(new UnitDef("santa", 8, 8, 4, .1f, extractPortrait(9)));
            AddDef(new UnitDef("snowman", 8, 2, 4, .1f, extractPortrait(2)));

            AddDef(new AnimatedGameObjectDef("snowball", "snowball"));
            AddDef(new AnimatedGameObjectDef("snow", "snow"));

            MainCamera.ZoomIn(1f);

            var mainMenuFonts = new SpriteFont[]
            {
                ContentManager.BakeFont("Whacky_Joe", 64),
                ContentManager.BakeFont("Direct_Message", 48),
                ContentManager.BakeFont("Tomorrow_Night", 16),
            };

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

            WindowManager = new WindowManager(new Theme[] {
                new("main menu", windowTextures[0], 32, 32, mainMenuFonts) { ControlDrawOffset = new(6, 6), TextColor = Color.White, TextMouseOverColor = Color.Yellow },
                new("stats window", windowTextures[0], 32, 32, gameFonts) { ControlDrawOffset = new(6, 6), TextColor = Color.White, TextMouseOverColor = Color.Yellow },
            });

            MainMenu = new MainMenu(WindowManager.DefaultTheme, WindowSize) { Clicked = MainMenu_Clicked };
            WindowManager.AddWindow(MainMenu);

            StatsWindow = WindowManager.NewWindow(0, 416 * 2, 480 * 2, 64 * 2, "stats window");
            StatsWindowCamera = new OrthographicCamera(GraphicsDevice) { Origin = MainCamera.Origin };
            StatsWindow.Hide();

            ContentManager.LoadAnimatedSpriteSheet(Snowball.AnimationKey);
        }

        protected override void PostLoad()
        {
            for (var index = 0; index < 8; index++)
                SpawnUnit(Random.Next(0, 3) switch
                {
                    1 => "elf1",
                    2 => "elf2",
                    _ => "elf3"
                }, Random.Next(4, 26), Random.Next(21, 29));
        }

        private void MainMenu_Clicked(Window _, string option)
        {
            switch (option)
            {
                case "New Game":
                    HideMenu();
                    NewGame();
                    break;
                case "Exit":
                    ShouldQuit = true;
                    break;
            }
        }

        private void HideMenu()
            => MainMenu.Hide();

        private Unit SpawnUnit(string unitDef, int cellX, int cellY, string state = Unit.States.Idle)
        {
            var unit = new Unit(GetDef<UnitDef>(unitDef), cellX * TileWidth, cellY * TileHeight, state);
            AddObject(unit);
            return unit;
        }

        private void NewGame()
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
            MainMap = new TiledMooseMap(Content.Load<TiledMap>($"Maps/{mapName}"));
            GetRenderer<TiledMooseMapRenderer>("map").Load(MainMap);
        }

        public static Snowball? SpawnSnowball(IEnumerable<Vector2> flightPath)
        {
            if (!flightPath.Any())
                return null;
            var snowBall = new Snowball(Instance.SnowballDef, flightPath);
            Instance.AddObject(snowBall);
            return snowBall;
        }

        protected override bool PreRenderUpdate(GameTime gameTime)
        {
            HandleInput();
            return true;
        }

        protected override void PostObjectsUpdate(GameTime gameTime)
        {
            WindowManager.Update(gameTime, CurrentMouseState);
            if (Mode == GameMode.Demo)
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
                    var startCell = SelectedUnit.GetCell();
                    var endCell = new Vector2(startCell.X + Random.Next(-3, 4), startCell.Y + Random.Next(-3, 4));
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
            WindowManager.Draw(SpriteBatch);
            SpriteBatch.End();
        }

        private void HandleInput()
        {
            if (Mode == GameMode.Demo)
                return;

            if (WasKeyJustPressed(Keys.Escape))
            { 
                switch (Mode)
                {
                    case GameMode.Paused:
                        Mode = GameMode.Playing;
                        MainMenu.Hide();
                        break;
                    case GameMode.Playing:
                        Mode = GameMode.Paused;
                        MainMenu.Show();
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
                    var unitCell = SelectedUnit.GetCell();
                    var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
                    mouseCell /= TileSize;

                    var path = MainMap.FindCellPath(unitCell, mouseCell);
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

            var displayNameSize = StatsWindow.MeasureString(unit.DisplayName);
            var smallFontHeight = (int)StatsWindow.MeasureString("|", 1).Y;

            StatsWindow.AddPicture(4, 4, unit.Portrait, scale: new(6, 6));
            var nameLabel = StatsWindow.AddLabel(113, 2, unit.DisplayName, color: Color.Yellow);
            var yOffset = (int)displayNameSize.Y + 2;
            StatsWindow.AddLine(112, yOffset, 112 + (int)displayNameSize.X, yOffset);
            StatsWindow.AddLabel(112, yOffset + 2, $"Speed: {unit.DisplaySpeed}", 1);
            StatsWindow.AddLabel(112, yOffset + 2 + smallFontHeight * 1, $"HP: {unit.DisplayHealth}/{unit.DisplayMaxHealth}", 1);
            StatsWindow.AddLabel(112, yOffset + 2 + smallFontHeight * 2, $"Accuracy: {unit.DisplayAccuracy}", 1);

            var unitCell = unit.GetCell();
            var cachedGrid = MainMap.BuildCollisionGrid(unitCell);
            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                {
                    var deltaCell = new Vector2(x, y);

                    if (SelectedUnitHintCells.ContainsKey(deltaCell * TileSize))
                        continue;

                    if (MainMap.GetBlockingVector(x, y).Any(b => b > 0))
                        continue;

                    var path = MainMap.FindCellPath(unitCell, deltaCell, cachedGrid);

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

                        SelectedUnitHintCells[p * TileSize] = color;
                    }
                }
        }

        private void ClearSelectUnits()
        {
            SelectedUnit = null;
            SelectedUnitHintCells.Clear();
            TargettedUnit = null;
            StatsWindow.Controls.Clear();
            StatsWindow.Hide();
        }

        private void DrawTargetLine()
        {
            if (TargettedUnit == null || SelectedUnit == null)
                return;

            var selectedUnitCell = SelectedUnit.GetCell();
            var targettedUnitCell = TargettedUnit.GetCell();

            var startWorldPosition = SelectedUnit.WorldPosition + HalfTileSize;
            var endWorldPosition = TargettedUnit.WorldPosition + HalfTileSize;
            var leftCone = endWorldPosition.RotateAround(startWorldPosition, 5).GetFloor();
            var rightCone = endWorldPosition.RotateAround(startWorldPosition, -5).GetFloor();

            void drawLineTo(Vector2 start, Vector2 end, Color color, bool extend, int thickness)
            {
                foreach (var (worldPosition, blockedVector) in MainMap.FindWorldRay(start, end, extend: extend))
                {
                    SpriteBatch.DrawPoint(worldPosition, color, thickness);
                    var cell = (worldPosition / TileSize).GetFloor();

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

            SpriteBatch.FillRectangle(SelectedUnit.WorldPosition, TileSize, Color.Red.HalveAlphaChannel());

            SelectedUnitHintCells.ForEach(t =>
            {
                SpriteBatch.DrawRectangle(new(t.Key.X, t.Key.Y, TileWidth + 1, TileHeight + 1), Color.LightGray);
                SpriteBatch.FillRectangle(new(t.Key.X + 1, t.Key.Y + 1, TileWidth - 1, TileHeight - 1), t.Value);
            });

            var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
            mouseCell /= TileSize;

            var unitCell = SelectedUnit.GetCell();
            var mousePath = MainMap.FindCellPath(unitCell, mouseCell);
            var mousePathCount = mousePath.Count();
            if (mousePathCount > 0 && mousePathCount <= SelectedUnit.DisplaySpeed)
            {
                var lastCell = unitCell * TileSize + HalfTileSize;
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
        }
    }
}
