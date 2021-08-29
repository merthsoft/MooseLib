using Merthsoft.MooseEngine;
using Merthsoft.MooseEngine.BaseDriver.Renderers;
using Merthsoft.MooseEngine.TiledDriver;
using MonoGame.Extended.Tiled;

namespace Merthsoft.CyberpunkRl
{
    public class Cprl : MooseGame
    {
        protected readonly List<TiledMooseMap> Rooms = new();
        protected readonly List<TiledMooseMap> Hallways = new();

        public Cprl()
        {
        }

        protected override void Load()
        {
            AddRenderer(MooseEngine.TiledDriver.DefaultRenderKeys.TiledMooseMapRenderer, new TiledMooseMapRenderer(GraphicsDevice));
            AddRenderer(MooseEngine.BaseDriver.Renderers.DefaultRenderKeys.SpriteBatchObjectRenderer, new SpriteBatchObjectRenderer(SpriteBatch));

            Rooms.Add(new(Content.Load<TiledMap>("Maps/room1")));
            Rooms.Add(new(Content.Load<TiledMap>("Maps/room2")));
            Hallways.Add(new(Content.Load<TiledMap>("Maps/hall1")));

            MainMap = new TiledMooseMap("map", 22, 16, 16, 16);
            MainMap.CopyFromMap(Rooms[0], destX: 0, destY: 0);
            MainMap.CopyFromMap(Rooms[1], destX: 11, destY: 0);
            MainMap.CopyFromMap(Hallways[0], destX: 0, destY: 11);

            MainCamera.ZoomIn(1f);
        }
    }
}
