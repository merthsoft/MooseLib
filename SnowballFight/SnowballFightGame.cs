using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MooseLib;
using MooseLib.Ui;
using Roy_T.AStar.Grids;
using System.Collections.Generic;
using System.Linq;

namespace SnowballFight
{
    public class SnowballFightGame : MooseGame
    {
        public int UnitLayer { get; private set; } = 2;
        public int SnowballLayer { get; private set; } = 4;

        private WindowManager WindowManager = null!;
        
        private Window StatsWindow = null!;
        public OrthographicCamera StatsWindowCamera = null!;

        private readonly List<(Vector2 worldPosition, Color color)> SelectedUnitHintCells = new();
        private Unit? SelectedUnit { get; set; }
        private Unit? TargettedUnit { get; set; }

        private Texture2D UnitsTexture = null!;

        private Dictionary<string, UnitDef> UnitDefs = null!;

        public SnowballFightGame()
        {
        }

        private static UnitDef MakeUnitDef(string name, int maxHealth, int speed, float accuracySigma, Texture2D portrait, string? displayNameOverride = null)
            => new() {
                AnimationKey = name, DefName = name,
                DisplayName = displayNameOverride ?? name.UpperFirst(),
                MaxHealth = maxHealth, Speed = speed, AccuracySigma = accuracySigma,
                Portrait = portrait,
            };

        protected override void Initialize()
        {
            base.Initialize();
            
            Graphics.PreferredBackBufferWidth = 960;
            Graphics.PreferredBackBufferHeight = 960;
            Graphics.ApplyChanges();
        }

        private Unit SpawnUnit(string unitDef, int cellX, int cellY, string direction = Direction.None, string state = State.Idle)
        {
            var def = UnitDefs[unitDef];
            if (!AnimationSpriteSheets.ContainsKey(def.AnimationKey))
                LoadAnimatedSpriteSheet(def.AnimationKey);
            var unit = new Unit(this, def, cellX, cellY, direction, state);
            Objects.Add(unit);
            return unit;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            InitializeMap(30, 30, 16, 16);
            MainMap.CopyMap(Content.Load<TiledMap>("Maps/testmap"), 0, 0);
            LoadMap();
            UnitLayer = ObjectLayerIndices.First();
            SnowballLayer = ObjectLayerIndices.Last();

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

            UnitDefs = new Dictionary<string, UnitDef>()
            {
                ["deer"] = MakeUnitDef("deer", 6, 6, 1, extractPortrait(10)),
                ["elf1"] = MakeUnitDef("elf1", 4, 5, .2f, extractPortrait(3)),
                ["elf2"] = MakeUnitDef("elf1", 4, 5, .2f, extractPortrait(4)),
                ["elf3"] = MakeUnitDef("elf1", 4, 5, .2f, extractPortrait(5)),
                ["krampus"] = MakeUnitDef("krampus", 8, 4, .1f, extractPortrait(8)),
                ["mari"] = MakeUnitDef("mari", 3, 11, .4f, extractPortrait(1), "Mari Lwyd"),
                ["santa"] = MakeUnitDef("santa", 8, 4, .1f, extractPortrait(9)),
                ["snowman"] = MakeUnitDef("snowman", 8, 4, .1f, extractPortrait(2)),
            };

            SpawnUnit("deer", 5, 9);
            SpawnUnit("elf1", 10, 5);
            SpawnUnit("elf2", 3, 10);
            SpawnUnit("elf3", 11, 11);

            SpawnUnit("krampus", 12, 14);
            SpawnUnit("mari", 13, 14);

            SpawnUnit("santa", 13, 12);
            SpawnUnit("snowman", 11, 13);

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
                new("Candycane", windowTextures[0], 32, 32, fonts) { ControlDrawOffset = new(6, 6), TextColor = Color.Gold, TextMouseOverColor = Color.Maroon },
            });

            StatsWindow = WindowManager.NewWindow(0, 416 * 2, 480 * 2, 64 * 2);
            StatsWindowCamera = new OrthographicCamera(GraphicsDevice) { Origin = MainCamera.Origin };

            LoadAnimatedSpriteSheet(Snowball.AnimationKey);
        }

        protected override void PreMapUpdate(GameTime gameTime)
            => HandleMouseInput();

        protected override void PostUpdate(GameTime gameTime) 
            => WindowManager.Update(gameTime, CurrentMouseState, WorldMouse);

        private void HandleMouseInput()
        {
            var mouseOverUnit = (UnitAtWorldLocation(WorldMouse) as Unit)!;

            if (CurrentMouseState.LeftButton.JustPressed(PreviousMouseState.LeftButton))
                HandleLeftClick();
            else if (CurrentMouseState.RightButton.JustPressed(PreviousMouseState.RightButton))
                HandleRightClick();
            else
            {
                if (SelectedUnit != mouseOverUnit)
                    TargettedUnit = mouseOverUnit;
            }

            void HandleRightClick()
            {
                if (!SelectedUnit?.AtWorldLocation(WorldMouse) ?? false)
                    ClearSelectUnits();
            }

            void HandleLeftClick()
            {
                if (SelectedUnit == null)
                {
                    if (mouseOverUnit != null && mouseOverUnit.State == State.Idle)
                    {
                        SelectSingleUnit(mouseOverUnit);
                        var unitCell = mouseOverUnit.GetCell();
                        Grid? cachedGrid = BuildCollisionGrid(unitCell);
                        for (var deltaX = -mouseOverUnit.DisplaySpeed; deltaX <= mouseOverUnit.DisplaySpeed; deltaX++)
                            for (var deltaY = -mouseOverUnit.DisplaySpeed; deltaY <= mouseOverUnit.DisplaySpeed; deltaY++)
                            {
                                var deltaCell = new Vector2(unitCell.X + deltaX, unitCell.Y + deltaY);

                                var path = FindCellPath(unitCell, deltaCell, cachedGrid);
                                var pathCount = path.Count();
                                if (pathCount > 0 && pathCount <= mouseOverUnit.DisplaySpeed)
                                {
                                    var color = pathCount - 1 <= mouseOverUnit.DisplaySpeed / 2
                                       ? Color.Green.HalveAlphaChannel()
                                       : Color.DarkOrange.HalveAlphaChannel();
                                    var worldDelta = mouseOverUnit.Position + new Vector2(deltaX * TileHeight, deltaY * TileWidth);
                                    SelectedUnitHintCells.Add((worldDelta, color));
                                }
                            }
                    }
                }
                else if (TargettedUnit == null)
                {
                    var unitCell = SelectedUnit.GetCell();
                    var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
                    mouseCell /= TileSize;

                    var path = FindCellPath(unitCell, mouseCell);
                    if (path.Any() && path.Count() <= SelectedUnit.DisplaySpeed)
                    {
                        path.ForEach(SelectedUnit.MoveQueue.Enqueue);
                        SelectedUnit.State = State.Walk;
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
        }

        private void ClearSelectUnits()
        {
            SelectedUnit = null;
            SelectedUnitHintCells.Clear();
            TargettedUnit = null;
            StatsWindow.Controls.Clear();
        }

        protected override void Draw(GameTime gameTime)
        {
            Draw(null, null,
                (_ => DrawSelectedUnitDetails(), null),
                null,
                (null, _ => DrawTargetLine())
            );

            SpriteBatch.Begin(transformMatrix: StatsWindowCamera.GetViewMatrix(), blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            WindowManager.Draw(SpriteBatch);
            SpriteBatch.End();
        }

        private void DrawTargetLine()
        {
            if (TargettedUnit != null && SelectedUnit != null)
            {
                var selectedUnitCell = SelectedUnit.GetCell();
                var targettedUnitCell = TargettedUnit.GetCell();

                var startWorldPosition = SelectedUnit.Position + HalfTileSize;
                var endWorldPosition = TargettedUnit.Position + HalfTileSize;
                var leftCone = endWorldPosition.RotateAround(startWorldPosition, 5).GetFloor();
                var rightCone = endWorldPosition.RotateAround(startWorldPosition, -5).GetFloor();

                void drawLineTo(Vector2 start, Vector2 end, Color color, bool extend, int thickness)
                {
                    foreach (var (worldPosition, blockedVector) in FindWorldRay(start, end, extend: extend))
                    {
                        SpriteBatch.DrawPoint(worldPosition, color, thickness);
                        var cell = (worldPosition / TileSize).GetFloor();

                        if (blockedVector.Skip(2).Sum() > 0 && cell != selectedUnitCell && cell != targettedUnitCell)
                            break;
                    }
                }

                drawLineTo(startWorldPosition, endWorldPosition, Color.Green, false, 3);
                drawLineTo(startWorldPosition, leftCone, Color.Red, true, 1);
                drawLineTo(startWorldPosition, rightCone, Color.Red, true, 1);
            }
        }

        private void DrawSelectedUnitDetails()
        {
            if (SelectedUnit == null)
                return;

            SpriteBatch.FillRectangle(SelectedUnit.Position, TileSize, Color.Red.HalveAlphaChannel());

            SelectedUnitHintCells.ForEach(t => SpriteBatch.FillRectangle(t.worldPosition, TileSize, t.color));

            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                    if (!MainMap.IsBlockedAt(x, y))
                        SpriteBatch.DrawRectangle(x * TileWidth, y * TileHeight, TileWidth + 1, TileHeight + 1, Color.LightGray);

            var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
            mouseCell /= TileSize;

            var unitCell = SelectedUnit.GetCell();
            var mousePath = FindCellPath(unitCell, mouseCell);
            var mousePathCount = mousePath.Count();
            if (mousePathCount > 0 && mousePathCount <= SelectedUnit.DisplaySpeed)
            {
                var lastCell = (unitCell * TileSize) + HalfTileSize;
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
