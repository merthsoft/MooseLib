using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MooseLib;

namespace CyberpunkRl
{
    public class Cprl : MooseGame
    {
        protected readonly List<TiledMap> Rooms = new();
        protected readonly List<TiledMap> Hallways = new();

        public Cprl()
        {
            Content.RootDirectory = nameof(Content);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InitializeMap(22, 16, 16, 16);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Rooms.Add(Content.Load<TiledMap>("Maps/room1"));
            Rooms.Add(Content.Load<TiledMap>("Maps/room2"));
            Hallways.Add(Content.Load<TiledMap>("Maps/hall1"));

            MainMap.CopyMap(Rooms[0], 0, 0);
            MainMap.CopyMap(Rooms[1], 11, 0);
            MainMap.CopyMap(Hallways[0], 0, 11);

            AddUnit("Adam", 5, 9, Direction.Down);
            AddUnit("Alex", 9, 4, Direction.Left);
            AddUnit("Bob", 1, 3, Direction.Up);

            base.LoadContent();

            MainCamera.ZoomIn(1f);
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            var worldClick = MainCamera.ScreenToWorld(mouseState.Position.X, mouseState.Position.Y);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var unit in Units)
                {
                    if (unit.AtWorldLocation(worldClick))
                    {
                        SelectedUnits.Clear();
                        SelectedUnits.Add(unit);
                        break;
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if ((SelectedUnits.Count == 1) && !SelectedUnits[0].AtWorldLocation(worldClick))
                    SelectedUnits.Clear();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
            => Draw(DrawSelectedUnitStuffPre, DrawSelectedUnitStuffPost);

        public void DrawSelectedUnitStuffPre(UnitBase obj)
        {

        }

        private void DrawSelectedUnitStuffPost(UnitBase obj)
        {

        }
    }
}
