using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.Miner
{
    public class MineLayer : ITileLayer<Tile>
    {
        public string Name { get; } = "mine";
        public bool IsHidden { get; set; }

        public int Width => MinerGame.BaseMapWidth;
        public int Height => MinerGame.BaseMapHeight;

        public Vector2 DrawOffset { get; }

        private readonly Tile[,] dirtMap = new Tile[MinerGame.BaseMapWidth, MinerGame.BaseMapHeight];
        private readonly Tile?[,] interactiveMap = new Tile?[MinerGame.BaseMapWidth, MinerGame.BaseMapHeight];

        public MineLayer()
            => Reset();

        public void Reset()
        {
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < 3; j++)
                {
                    interactiveMap[i, j] = Tile.Sky_Blocking;
                    dirtMap[i, j] = Tile.Empty;
                }

            for (var i = 0; i < Width; i++)
            {
                interactiveMap[i, 3] = Tile.Sky;
                interactiveMap[i, 4] = i > 0 ? Tile.MineCeiling : Tile.ElevatorShaft;
                dirtMap[i, 3] = Tile.Empty;
                dirtMap[i, 4] = Tile.Empty;
            }

            for (var i = 0; i < Width; i++)
                for (var j = 5; j < Height; j++)
                {
                    dirtMap[i, j] = i > 0 ? Tile.Dirt : Tile.Empty;
                    interactiveMap[i, j] = i > 0 ? null : Tile.ElevatorShaft;
                }
        }

        public bool CellIsInBounds(int cellX, int cellY)
            => cellX >= 0 && cellX < Width
            && cellY >= 0 && cellY < Height;

        public Tile TileAt(int x, int y)
            => interactiveMap[x, y] switch
            {
                null => Tile.Empty,
                var interactiveTile when interactiveTile.Value.HasFlag(Tile.Flag_Hidden) => Tile.Stone,
                var interactiveTile => interactiveTile.Value & ~Tile.Flag_Hidden
            };
        
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
                if (tile.Value.HasFlag(Tile.Flag_Hidden))
                    tile = interactiveMap[x, y] &= (~Tile.Flag_Hidden);
                if (tile < Tile.Sky)
                {
                    dirtMap[x, y] = Tile.Empty;
                    interactiveMap[x, y] = Tile.Empty;

                    return tile.Value;
                }

                return tile!.Value;
            }

            var revealedTile = GetRevelation();

            if (revealedTile < Tile.Sky)
            {
                dirtMap[x, y] = Tile.Empty;
                interactiveMap[x, y] = Tile.Empty;
            }
            else
                interactiveMap[x, y] = revealedTile;
            return revealedTile;
        }

        private static Tile GetRevelation()
        {
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

            return value;
        }

        public void SearchCell(int x, int y, bool torch, bool lantern)
        {
            if (!CellIsInBounds(x, y))
                return;

            for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    if (!CellIsInBounds(x + i, y + j))
                        continue;

                    var roll = Random.Shared.Next(0, 100) + (lantern ? 10 : torch ? 4 : 0);
                    if (roll <= 90)
                        continue;

                    var tile = interactiveMap[x + i, y + j];

                    if (tile.HasValue)
                    {
                        if (tile.Value.HasFlag(Tile.Flag_Hidden) && Random.Shared.NextDouble() >= .65)
                            interactiveMap[x + i, y + j] &= ~Tile.Flag_Hidden;

                        break;
                    }

                    var revelation = GetRevelation();

                    if (revelation != Tile.Empty && Random.Shared.NextDouble() < .65)
                        revelation |= Tile.Flag_Hidden;

                    interactiveMap[x + i, y + j] = revelation;
                    
                    break;
                }
        }

        public ITile<Tile> GetTile(int x, int y)
            => new SimpleTileReference<Tile>(GetTileValue(x, y));

        public Tile GetTileValue(int x, int y)
            => TileAt(x, y);
    }
}
