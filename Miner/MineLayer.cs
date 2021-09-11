using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.Miner
{
    class MineLayer : ITileLayer<int>
    {
        public string Name { get; } = "mine";
        public bool IsHidden { get; set; }

        public int Width => MinerGame.BaseMapWidth;
        public int Height => MinerGame.BaseMapHeight;

        public Vector2 DrawOffset { get; }

        private readonly Tile[,] dirtMap = new Tile[MinerGame.BaseMapWidth, MinerGame.BaseMapHeight];
        private readonly Tile?[,] interactiveMap = new Tile?[MinerGame.BaseMapWidth, MinerGame.BaseMapHeight];

        public MineLayer()
        {
            Reset();
        }

        public void Reset()
        {
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                {
                    dirtMap[i, j] = (Tile)(Random.Shared.Next(3) + 1);
                    interactiveMap[i, j] = null;
                }

            dirtMap[0, 0] = Tile.Empty;
            interactiveMap[0, 0] = Tile.Empty;
        }

        public bool CellIsInBounds(int cellX, int cellY)
            => cellX >= 0 && cellX < Width
            && cellY >= 0 && cellY < Height;

        public Tile TileAt(int x, int y)
            => interactiveMap[x, y] ?? dirtMap[x, y];

        public Tile GetDirtTile(int x, int y)
            => dirtMap[x, y];

        public Tile GetInteractiveTile(int x, int y)
            => interactiveMap[x, y] ?? Tile.Empty;

        public Tile Mine(int x, int y)
        {
            if (!CellIsInBounds(x, y))
                return Tile.Border;

            var tile = interactiveMap[x, y];

            if (tile.HasValue)
            {
                if (tile <= Tile.LastMinable)
                {
                    dirtMap[x, y] = Tile.Empty;
                    interactiveMap[x, y] = Tile.Empty;

                    return tile.Value;
                }

                return Tile.Border;
            }

            var value = Tile.Empty;
            if (Random.Shared.NextDouble() < .35)
            {
                var roll = Random.Shared.NextDouble();

                value = roll switch
                {
                    < .15 => Tile.Granite,
                    < .35 => Tile.Silver,
                    < .45 => Tile.VolcanicRock,
                    < .55 => Tile.SandStone,
                    
                    < .60 => Tile.CaveIn,
                    < .65 => Tile.Flood,
                    
                    < .70 => Tile.Water,
                    
                    < .80 => Tile.Gold,
                    < .90 => Tile.Platinum,
                    
                    < .95 => Tile.Diamond,
                    _ => Tile.Clover
                };
            }

            if (value <= Tile.LastMinable)
            {
                dirtMap[x, y] = Tile.Empty;
                interactiveMap[x, y] = Tile.Empty;
            }
            else
                interactiveMap[x, y] = value;
            return value;
        }

        public void SearchCell(int x, int y, bool torch, bool lantern)
        {
            if (!CellIsInBounds(x, y))
                return;
        }

        public ITile<int> GetTile(int x, int y)
            => new SimpleTileReference<int>(GetTileValue(x, y));

        public int GetTileValue(int x, int y)
            => (int)TileAt(x, y);
    }
}
