using EpPathFinding.cs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MooseLib;
using System;
using System.Diagnostics;

namespace SnowballFight
{
    public class SnowballFightGame : MooseGame
    {
        private MouseState CurrentMouseState;

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
                foreach (var unit in Units)
                {
                    if (unit.Clicked(worldClick))
                    {
                        SelectedUnits.Clear();
                        SelectedUnits.Add(unit);
                        break;
                    }
                }
            }

            if (CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                if (SelectedUnits.Count == 1 && !SelectedUnits[0].Clicked(worldClick))
                    SelectedUnits.Clear();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
            => Draw(
                selectedUnitPreDraw: DrawSelectedUnitStuffPre,
                selectedUnitPostDraw: DrawSelectedUnitStuffPost);

        private void DrawSelectedUnitStuffPre(Unit selectedUnit)
            => SpriteBatch.FillRectangle(selectedUnit.Location, TileSize, Color.Red.HalveAlphaChannel());

        private void DrawSelectedUnitStuffPost(Unit selectedUnit)
        {
            var unitCell = selectedUnit.GetCell(MainMap);
            WalkableGrid.SetWalkableAt((int)unitCell.X, (int)unitCell.Y, true);

            var pathFinder = new JumpPointParam(WalkableGrid, iDiagonalMovement: DiagonalMovement.Never, iAllowEndNodeUnWalkable: EndNodeUnWalkableTreatment.DISALLOW, iMode: HeuristicMode.MANHATTAN);
            
            for (var deltaX = -selectedUnit.Speed; deltaX <= selectedUnit.Speed; deltaX++)
                for (var deltaY = -selectedUnit.Speed; deltaY <= selectedUnit.Speed; deltaY++)
                {
                    var deltaCell = new Vector2(unitCell.X + deltaX, unitCell.Y + deltaY);

                    if (deltaCell.X < 0 || deltaCell.Y < 0 || deltaCell.X >= MapWidth || deltaCell.Y >= MapHeight)
                        continue;

                    var path = pathFinder.FindPath(unitCell, deltaCell);
                    if (path.Count > 0 && path.Count - 1 <= selectedUnit.Speed)
                    {
                        var color = path.Count - 1 <= selectedUnit.Speed / 2
                                    ? Color.Green.HalveAlphaChannel()
                                    : Color.DarkOrange.HalveAlphaChannel();
                        var worldDelta = selectedUnit.Location + new Vector2(deltaX * TileHeight, deltaY * TileWidth);
                        SpriteBatch.FillRectangle(worldDelta, TileSize, color);
                    }
                }

            for (var x = 0; x < MapWidth; x++)
                for (var y = 0; y < MapHeight; y++)
                    if (!MainMap.IsBlockedAt(x, y))
                        SpriteBatch.DrawRectangle(x * TileWidth, y * TileHeight, TileWidth + 1, TileHeight + 1, Color.White);

            var mouseCell = MainCamera.ScreenToWorld(
                                CurrentMouseState.Position.X / TileWidth * TileWidth,
                                CurrentMouseState.Position.Y / TileHeight * TileHeight);
            mouseCell /= TileSize;

            var mousePath = pathFinder.FindPath(unitCell, mouseCell);
            if (mousePath.Count > 0 && mousePath.Count - 1 <= selectedUnit.Speed)
            {
                var lastCell = (unitCell * TileSize) + HalfTileSize;
                foreach (var p in mousePath)
                {
                    var nextCell = new Vector2(p.x * TileWidth, p.y * TileHeight) + HalfTileSize;
                    SpriteBatch.DrawLine(lastCell, nextCell, Color.Black, 2);
                    lastCell = nextCell;
                }

            }

            WalkableGrid.SetWalkableAt((int)unitCell.X, (int)unitCell.Y, false);
        }
    }
}
