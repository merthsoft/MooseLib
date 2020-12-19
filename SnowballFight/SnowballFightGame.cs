using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MooseLib;
using MooseLib.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SnowballFight
{
    public class SnowballFightGame : MooseGame
    {
        private MouseState CurrentMouseState;
        private WindowManager WindowManager = null!;

        private readonly List<(Vector2, Color)> SelectedUnitHintCells = new();
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

        protected override void LoadContent()
        {
            MainMap.CopyMap(Content.Load<TiledMap>("Maps/testmap"), 0, 0);

            AddUnitToSpawnQueue("deer", 5, 9, speed: 3);
            AddUnitToSpawnQueue("elf1", 9, 4, speed: 3);
            AddUnitToSpawnQueue("elf2", 1, 3, speed: 3);
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
            var mouseOverUnit = UnitAtWorldLocation(worldClick);

            if (CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (SelectedUnits.Count == 0)
                {
                    if (mouseOverUnit == null)
                        return;
                    
                    SelectSingleUnit(mouseOverUnit);
                    var unitCell = mouseOverUnit.GetCell();
                    for (var deltaX = -mouseOverUnit.Speed; deltaX <= mouseOverUnit.Speed; deltaX++)
                        for (var deltaY = -mouseOverUnit.Speed; deltaY <= mouseOverUnit.Speed; deltaY++)
                        {
                            var deltaCell = new Vector2(unitCell.X + deltaX, unitCell.Y + deltaY);

                            var path = FindPath(unitCell, deltaCell);
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
            else if (CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                if (SelectedUnits.Count == 1 && !SelectedUnits[0].AtWorldLocation(worldClick))
                    ClearSelectUnits();
            }
            else
            {
                if (SelectedUnits.Count == 1 && SelectedUnits[0] != mouseOverUnit)
                    TargettedUnit = mouseOverUnit;
            }

            base.Update(gameTime);
            WindowManager.Update(gameTime, MainCamera);
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
            TargettedUnit = null;
        }

        protected override void Draw(GameTime gameTime)
        {
            Draw(DrawSelectedUnitStuffPre, DrawSelectedUnitStuffPost);
            SpriteBatch.Begin(transformMatrix: MainCamera.GetViewMatrix(), blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            WindowManager.Draw(gameTime, SpriteBatch);
            SpriteBatch.End();
        }

        private void DrawSelectedUnitStuffPost(Unit selectedUnit)
        {
            if (TargettedUnit != null)
                SpriteBatch.DrawLine(selectedUnit.Position + HalfTileSize, TargettedUnit.Position + HalfTileSize, Color.DarkRed, 3);
        }

        private void DrawSelectedUnitStuffPre(Unit selectedUnit)
        {
            SpriteBatch.FillRectangle(selectedUnit.Position, TileSize, Color.Red.HalveAlphaChannel());

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
            if (mousePathCount > 0 && mousePathCount <= selectedUnit.Speed)
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
