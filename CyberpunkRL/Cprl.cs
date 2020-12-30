using System.Collections.Generic;
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

        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            Rooms.Add(Content.Load<TiledMap>("Maps/room1"));
            Rooms.Add(Content.Load<TiledMap>("Maps/room2"));
            Hallways.Add(Content.Load<TiledMap>("Maps/hall1"));

            InitializeMap(22, 16, 16, 16);
            MainMap.CopyMap(Rooms[0], 0, 0);
            MainMap.CopyMap(Rooms[1], 11, 0);
            MainMap.CopyMap(Hallways[0], 0, 11);

            MainCamera.ZoomIn(1f);
        }
    }
}
