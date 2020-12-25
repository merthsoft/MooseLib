using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MooseLib;
using MooseLib.Ui;
using System.Collections.Generic;
using System.Linq;

namespace SnowballFight
{
    public class SnowballFightGame : MooseGame
    {
        private const int UnitLayer = 2;

        private MouseState CurrentMouseState;
        private WindowManager WindowManager = null!;

        private readonly Queue<GameObject> SpawnQueue = new();
        private readonly List<(Vector2, Color)> SelectedUnitHintCells = new();
        private Unit? SelectedUnit { get; set; }
        private Unit? TargettedUnit { get; set; }

        public SnowballFightGame()
        {
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = 960;
            Graphics.PreferredBackBufferHeight = 960;
            Graphics.ApplyChanges();

            InitializeMap(30, 30, 16, 16);
            base.Initialize();
        }

        private Unit AddUnitToSpawnQueue(string animationKey, int cellX, int cellY, string direction = Direction.None, string state = State.Idle, int speed = 0)
        {
            if (!Animations.ContainsKey(animationKey))
                LoadAnimation(animationKey);
            var unit = new Unit(this, Animations[animationKey], cellX, cellY, direction, state, UnitLayer) { Speed = speed };
            SpawnQueue.Enqueue(unit);
            return unit;
        }

        protected override void LoadContent()
        {
            MainMap.CopyMap(Content.Load<TiledMap>("Maps/testmap"), 0, 0);

            AddUnitToSpawnQueue("deer", 5, 9, speed: 3);
            AddUnitToSpawnQueue("elf1", 10, 5, speed: 3);
            AddUnitToSpawnQueue("elf2", 3, 10, speed: 3);
            AddUnitToSpawnQueue("elf3", 11, 11, speed: 3);

            AddUnitToSpawnQueue("krampus", 12, 14, speed: 4);
            AddUnitToSpawnQueue("mari", 13, 14, speed: 4);

            AddUnitToSpawnQueue("santa", 13, 12, speed: 5);
            AddUnitToSpawnQueue("snowman", 11, 13, speed: 5);

            base.LoadContent();

            MainCamera.ZoomIn(1f);

            var fonts = new SpriteFont[]
            {
                Content.Load<SpriteFont>("Fonts/TheKingIsDead"),
                Content.Load<SpriteFont>("Fonts/gothic-pixel-font"),
                Content.Load<SpriteFont>("Fonts/Pixeled English Font"),
            };

            var windowTextures = new Texture2D[]
            {
                Content.Load<Texture2D>("Images/window1"),
                Content.Load<Texture2D>("Images/window2"),
            };

            WindowManager = new WindowManager(new Theme[] {
                new("Candycane", windowTextures[0], 16, 16, fonts[0]) { ControlDrawOffset = new(6, 6), TextColor = Color.Gold, TextMouseOverColor = Color.Maroon },
                new("Winter", windowTextures[1], 16, 16, fonts[0]) { ControlDrawOffset = new(6, 6), TextColor = Color.Gold, TextMouseOverColor = Color.Maroon }
            });
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentMouseState = Mouse.GetState();
            var worldClick = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
            var mouseOverUnit = (UnitAtWorldLocation(worldClick) as Unit)!;

            if (CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (SelectedUnit == null)
                {
                    if (mouseOverUnit != null && mouseOverUnit.State == State.Idle)
                    {
                        SelectSingleUnit(mouseOverUnit);
                        var unitCell = mouseOverUnit.GetCell();
                        for (var deltaX = -mouseOverUnit.Speed; deltaX <= mouseOverUnit.Speed; deltaX++)
                            for (var deltaY = -mouseOverUnit.Speed; deltaY <= mouseOverUnit.Speed; deltaY++)
                            {
                                var deltaCell = new Vector2(unitCell.X + deltaX, unitCell.Y + deltaY);

                                var path = FindCellPath(unitCell, deltaCell);
                                var pathCount = path.Count();
                                if (pathCount > 0 && pathCount <= mouseOverUnit.Speed)
                                {

                                    var color = pathCount - 1 <= mouseOverUnit.Speed / 2
                                       ? Color.Green.HalveAlphaChannel()
                                       : Color.DarkOrange.HalveAlphaChannel();
                                    var worldDelta = mouseOverUnit.Position + new Vector2(deltaX * TileHeight, deltaY * TileWidth);
                                    SelectedUnitHintCells.Add((worldDelta, color));
                                }
                            }
                    }
                }
                else
                {
                    var unitCell = SelectedUnit.GetCell();
                    var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
                    mouseCell /= TileSize;

                    var path = FindCellPath(unitCell, mouseCell);
                    if (path.Any())
                    {
                        path.ForEach(SelectedUnit.MoveQueue.Enqueue);
                        SelectedUnit.State = State.Walk;
                        ClearSelectUnits();
                    }
                }
            } 
            else if (CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                if (!SelectedUnit?.AtWorldLocation(worldClick) ?? false)
                    ClearSelectUnits();
            }
            else
            {
                if (SelectedUnit != mouseOverUnit)
                    TargettedUnit = mouseOverUnit;
            }

            if (SpawnQueue.Count > 0)
                Objects.Add(SpawnQueue.Dequeue());

            base.Update(gameTime);
            WindowManager.Update(gameTime, MainCamera);
        }

        private void SelectSingleUnit(Unit unit)
        {
            ClearSelectUnits();
            SelectedUnit = unit;
        }

        private void ClearSelectUnits()
        {
            SelectedUnit = null;
            SelectedUnitHintCells.Clear();
            TargettedUnit = null;
        }

        protected override void Draw(GameTime gameTime)
        {
            Draw(null, null,
                (_ => DrawSelectedUnitDetails(), null),
                null,
                (null, _ => DrawTargetLine())
            );

            SpriteBatch.Begin(transformMatrix: MainCamera.GetViewMatrix(), blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            WindowManager.Draw(gameTime, SpriteBatch);
            SpriteBatch.End();
        }

        private void DrawTargetLine()
        {
            if (TargettedUnit != null && SelectedUnit != null)
            {
                var selectedUnitCell = SelectedUnit.GetCell();
                var targettedUnitCell = TargettedUnit.GetCell();

                foreach (var pos in FindWorldRay(SelectedUnit.Position + HalfTileSize, TargettedUnit.Position + HalfTileSize))
                {
                    SpriteBatch.DrawPoint(pos.worldPosition, Color.Green, 2);
                    var cell = (pos.worldPosition / TileSize).GetFloor();
                    
                    if (pos.blockedVector.Skip(2).Sum() > 0 && cell != selectedUnitCell && cell != targettedUnitCell)
                        break;
                }
            }
        }

        private void DrawSelectedUnitDetails()
        {
            if (SelectedUnit == null)
                return;

            SpriteBatch.FillRectangle(SelectedUnit.Position, TileSize, Color.Red.HalveAlphaChannel());

            SelectedUnitHintCells.ForEach(((Vector2 worldDelta, Color color) t) =>
                        SpriteBatch.FillRectangle(t.worldDelta, TileSize, t.color));

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
            if (mousePathCount > 0 && mousePathCount <= SelectedUnit.Speed)
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
