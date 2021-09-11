﻿using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.Miner
{
    class MineMap : BaseMap
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

        public override void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null)
        {
            
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
