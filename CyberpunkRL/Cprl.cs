using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MooseLib;
using System.Collections.Generic;

namespace CyberbunkRl
{
    public class Cprl : MooseGame
    {
        protected readonly List<TiledMap> Rooms = new();
        protected readonly List<TiledMap> Hallways = new();

        public Cprl()
        {
            Content.RootDirectory = "Content";
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

            Animations["Adam"] = Content.Load<SpriteSheet>("Characters/Adam.sf", new JsonContentLoader());
            Animations["Alex"] = Content.Load<SpriteSheet>("Characters/Alex.sf", new JsonContentLoader());
            Animations["Bob"] = Content.Load<SpriteSheet>("Characters/Bob.sf", new JsonContentLoader());

            AddPlayerUnit("Adam", 5, 9);
            AddPlayerUnit("Alex", 9, 4, Direction.Left);
            AddPlayerUnit("Bob", 1, 3, Direction.Up);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            var worldClick = MainCamera.ScreenToWorld(mouseState.Position.X, mouseState.Position.Y);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var unit in PlayerUnits)
                {
                    if (unit.Clicked(worldClick))
                    {
                        SelectedUnits.Clear();
                        SelectedUnits.Add(unit);
                        break;
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (SelectedUnits.Count == 1 && !SelectedUnits[0].Clicked(worldClick))
                    SelectedUnits.Clear();
            }

            base.Update(gameTime);
        }
    }
}
