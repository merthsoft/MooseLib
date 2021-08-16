using Merthsoft.MooseEngine;
using Merthsoft.MooseEngine.BaseDriver;
using Merthsoft.MooseEngine.Defs;
using Merthsoft.MooseEngine.TiledDriver;
using Merthsoft.MooseEngine.Ui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.SnowballFight
{
    public class SnowballFightGame : MooseGame
    {
        public const int WindowSize = 960;

        public static SnowballFightGame Instance { get; private set; } = null!;

        // TODO: Move layer values into something else
        public static int UnitLayer { get; private set; } = 2;

        public static int SnowballLayer { get; private set; } = 4;

        private WindowManager WindowManager = null!;
        private Window StatsWindow = null!;
        private SimpleMenu MainMenu = null!;

        public OrthographicCamera StatsWindowCamera = null!;

        private readonly Dictionary<Vector2, Color> SelectedUnitHintCells = new();

        private Unit? SelectedUnit { get; set; }

        private Unit? TargettedUnit { get; set; }

        private Texture2D UnitsTexture = null!;

        private IEnumerable<Unit> Units => ReadObjects.OfType<Unit>();

        public AnimatedGameObjectDef SnowballDef => (Defs["snowball"] as AnimatedGameObjectDef)!;

        public SnowballFightGame()
        {
            Instance = this;
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
            AddRenderer(TiledMooseMapRenderer.DefaultRenderKey, new TiledMooseMapRenderer(GraphicsDevice));
            AddRenderer(SpriteBatchObjectRenderer.DefaultRenderKey, new SpriteBatchObjectRenderer(SpriteBatch));

            MainMap = new TiledMooseMap(Content.Load<TiledMap>("Maps/testmap"));
            LoadMap();
            UnitLayer = MainMap.ObjectLayerIndices.First();
            SnowballLayer = MainMap.ObjectLayerIndices.Last();

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

            AddDef(new UnitDef("deer", 6, 6, 1, extractPortrait(10)));
            AddDef(new UnitDef("elf1", 4, 5, .2f, extractPortrait(3)));
            AddDef(new UnitDef("elf2", 4, 5, .2f, extractPortrait(4)));
            AddDef(new UnitDef("elf3", 4, 5, .2f, extractPortrait(5)));
            AddDef(new UnitDef("krampus", 8, 4, .1f, extractPortrait(8)));
            AddDef(new UnitDef("mari", 3, 11, .4f, extractPortrait(1), "Mari Lwyd"));
            AddDef(new UnitDef("santa", 8, 4, .1f, extractPortrait(9)));
            AddDef(new UnitDef("snowman", 8, 4, .1f, extractPortrait(2)));
            AddDef(new AnimatedGameObjectDef("snowball", "snowball"));

            MainCamera.ZoomIn(1f);

            var fonts = new SpriteFont[]
            {
                Content.Load<SpriteFont>("Fonts/Whacky_Joe_18"),
                Content.Load<SpriteFont>("Fonts/Direct_Message_14"),
            };

            var windowTextures = new Texture2D[]
            {
                Content.Load<Texture2D>("Images/window1"),
                Content.Load<Texture2D>("Images/window2"),
            };

            WindowManager = new WindowManager(new Theme[] {
                new("Candycane", windowTextures[0], 32, 32, fonts) { ControlDrawOffset = new(6, 6), TextColor = Color.White, TextMouseOverColor = Color.Maroon },
            });

            MainMenu = new SimpleMenu(WindowManager.DefaultTheme, "New Game", "Exit");
            WindowManager.AddWindow(MainMenu);

            MainMenu.Center(WindowSize, WindowSize);
            MainMenu.Clicked = MainMenu_Clicked;

            StatsWindow = WindowManager.NewWindow(0, 416 * 2, 480 * 2, 64 * 2);
            StatsWindowCamera = new OrthographicCamera(GraphicsDevice) { Origin = MainCamera.Origin };
            StatsWindow.Hide();

            ContentManager.LoadAnimatedSpriteSheet(Snowball.AnimationKey);
        }

        private void MainMenu_Clicked(string option)
        {
            switch (option)
            {
                case "New Game":
                    MainMenu.Hide();
                    NewGame();
                    break;
                case "Exit":
                    ShouldQuit = true;
                    break;
            }
        }

        private Unit SpawnUnit(string unitDef, int cellX, int cellY, string state = Unit.States.Idle)
        {
            var unit = new Unit(GetDef<UnitDef>(unitDef), cellX * TileWidth, cellY * TileHeight, state);
            AddObject(unit);
            return unit;
        }

        private void NewGame()
        {
            SpawnUnit("deer", 5, 9);
            SpawnUnit("elf1", 10, 5);
            SpawnUnit("elf2", 3, 10);
            SpawnUnit("elf3", 11, 11);

            SpawnUnit("krampus", 12, 14);
            SpawnUnit("mari", 13, 14);

            SpawnUnit("santa", 13, 12);
            SpawnUnit("snowman", 11, 13);
        }

        public static Snowball SpawnSnowball(IEnumerable<Vector2> flightPath)
        {     
            var snowBall = new Snowball(Instance.SnowballDef, flightPath);
            Instance.AddObject(snowBall);
            return snowBall;
        }

        protected override void PreRenderUpdate(GameTime gameTime)
            => HandleMouseInput();

        protected override void PostUpdate(GameTime gameTime)
            => WindowManager.Update(gameTime, CurrentMouseState, WorldMouse);

        protected override void Draw(GameTime gameTime)
        {
            Draw(null, null,
                (_ => DrawSelectedUnitDetails(), null),
                null,
                (null, _ => DrawTargetLine()));

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            WindowManager.Draw(SpriteBatch);
            SpriteBatch.End();
        }


        private void HandleMouseInput()
        {
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
            StatsWindow.AddLabel(113, 2, unit.DisplayName);
            var yOffset = (int)displayNameSize.Y - 4;
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
