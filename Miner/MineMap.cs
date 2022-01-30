using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.Miner
{
    public class MineMap : BaseMap
    {
        public MineLayer MineLayer => (Layers[0] as MineLayer)!;

        public override int Height => MinerGame.BaseMapWidth;
        public override int Width => MinerGame.BaseMapHeight;
        public override int TileWidth => MinerGame.BaseTileWidth;
        public override int TileHeight => MinerGame.BaseTileHeight;
        public override IReadOnlyList<ILayer> Layers { get; }

        public MineMap()
        {
            Layers = new ILayer[]
            {
                new MineLayer(),
                new ObjectLayer("player"),
            };
        }

        protected override int IsBlockedAt(int layer, int x, int y)
            => 0;

        public override void Update(GameTime gameTime)
        {
            
        }

        public Tile Mine(Vector2 cell)
            => MineLayer.Mine((int)cell.X, (int)cell.Y);

        public void SearchCell(Vector2 cell, bool torch, bool lantern)
            => MineLayer.SearchCell((int)cell.X, (int)cell.Y, torch, lantern);
    }
}
