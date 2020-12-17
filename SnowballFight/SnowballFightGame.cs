using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MooseLib;
using System.Collections.Generic;
using System.Linq;

namespace SnowballFight
{
    public class SnowballFightGame : MooseGame
    {
        private MouseState CurrentMouseState;
        private readonly List<(Vector2, Color)> SelectedUnitHintCells = new();

        public SnowballFightGame()
        {
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 768;
            Graphics.ApplyChanges();

            InitializeMap(20, 20, 16, 16);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            MainMap.CopyMap(Content.Load<TiledMap>("Maps/testmap"), 0, 0);

            AddUnitToSpawnQueue("Skeleton", 5, 9, speed: 3);
            AddUnitToSpawnQueue("Skeleton", 9, 4, speed: 3);
            AddUnitToSpawnQueue("Skeleton", 1, 3, speed: 3);
            AddUnitToSpawnQueue("Skeleton", 11, 11, speed: 3, state: State.Attack);

            AddUnitToSpawnQueue("Crossbowman", 12, 14, speed: 4);
            AddUnitToSpawnQueue("Crossbowman", 13, 14, speed: 4);

            AddUnitToSpawnQueue("Footman", 13, 12, speed: 5);
            AddUnitToSpawnQueue("Footman", 11, 13, speed: 5);
            var attackingFootman = AddUnitToSpawnQueue("Footman", 12, 12, speed: 5, state: State.Attack);
            attackingFootman.SpriteEffects = SpriteEffects.FlipHorizontally;

            AddUnitToSpawnQueue("Dog", 10, 13, speed: 7);

            base.LoadContent();

            MainCamera.ZoomIn(1f);
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentMouseState = Mouse.GetState();
            var worldClick = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);

            if (CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (SelectedUnits.Count == 0)
                {
                    foreach (var unit in Units)
                    {
                        if (unit.Clicked(worldClick))
                        {
                            SelectSingleUnit(unit);
                            var unitCell = unit.GetCell();
                            for (var deltaX = -unit.Speed; deltaX <= unit.Speed; deltaX++)
                                for (var deltaY = -unit.Speed; deltaY <= unit.Speed; deltaY++)
                                {
                                    var deltaCell = new Vector2(unitCell.X + deltaX, unitCell.Y + deltaY);

                                    var path = FindPath(unitCell, deltaCell);
                                    var pathCount = path.Count();
                                    if (pathCount > 0 && pathCount <= unit.Speed)
                                    {

                                        var color = pathCount - 1 <= unit.Speed / 2
                                           ? Color.Green.HalveAlphaChannel()
                                           : Color.DarkOrange.HalveAlphaChannel();
                                        var worldDelta = unit.Location + new Vector2(deltaX * TileHeight, deltaY * TileWidth);
                                        SelectedUnitHintCells.Add((worldDelta, color));
                                    }
                                }

                            break;
                        }
                    }
                }
                else
                {
                    var selectedUnit = SelectedUnits[0];
                    var unitCell = selectedUnit.GetCell();
                    var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
                    mouseCell /= TileSize;

                    var path = FindPath(unitCell, mouseCell);
                    if (path.Any())
                    {
                        path.ForEach(selectedUnit.MoveQueue.Enqueue);
                        selectedUnit.State = State.Walk;
                        ClearSelectUnits();
                    }
                }
            }

            if (CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                if (SelectedUnits.Count == 1 && !SelectedUnits[0].Clicked(worldClick))
                    ClearSelectUnits();
            }

            base.Update(gameTime);
        }

        private void SelectSingleUnit(Unit unit)
        {
            ClearSelectUnits();
            SelectedUnits.Add(unit);
        }

        private void ClearSelectUnits()
        {
            SelectedUnits.Clear();
            SelectedUnitHintCells.Clear();
        }

        protected override void Draw(GameTime gameTime)
        {
            Draw(DrawSelectedUnitStuffPre);
        }

        private void DrawSelectedUnitStuffPre(Unit selectedUnit)
        {
            SpriteBatch.FillRectangle(selectedUnit.Location, TileSize, Color.Red.HalveAlphaChannel());

            var unitCell = selectedUnit.GetCell();

            SelectedUnitHintCells.ForEach(((Vector2 worldDelta, Color color) t) =>
                        SpriteBatch.FillRectangle(t.worldDelta, TileSize, t.color));

            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                    if (!MainMap.IsBlockedAt(x, y))
                        SpriteBatch.DrawRectangle(x * TileWidth, y * TileHeight, TileWidth + 1, TileHeight + 1, Color.White);

            var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
            mouseCell /= TileSize;

            var mousePath = FindPath(unitCell, mouseCell);
            var mousePathCount = mousePath.Count();
            if (mousePathCount > 1 && mousePathCount <= selectedUnit.Speed)
            {
                var lastCell = (unitCell * TileSize) + HalfTileSize;
                var index = 0;
                foreach (var p in mousePath)
                {
                    var nextCell = new Vector2(p.X * TileWidth, p.Y * TileHeight) + HalfTileSize;
                    SpriteBatch.DrawLine(lastCell, nextCell, Color.Black, 2);
                    //if (index > 1)
                        SpriteBatch.DrawCircle(lastCell, 2, 40, Color.Red, 2);
                    lastCell = nextCell;
                    index++;
                }
                SpriteBatch.DrawCircle(lastCell, 2, 40, Color.Red, 2);
            }
        }
    }
}
