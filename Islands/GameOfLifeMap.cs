using Merthsoft.Moose.MooseEngine.BaseDriver;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.Islands
{
    class GameOfLifeMap : MultiMap<int>
    {
        private TimeSpan lastUpdate = TimeSpan.Zero;

        private int mapBuffer = 0;
        private readonly int[,,] map = new int[2, 80, 80];
        private readonly int[,] waterMap = new int[80, 80];

        public int this[int x, int y]
        {
            get => Get(x, y);
            set => Set(x, y, value);
        }

        public bool DoUpdate { get; set; } = false;

        public GameOfLifeMap(int width, int height, int tileWidth, int tileHeight)
            : base(width, height, tileWidth, tileHeight, LayerType.Tile, LayerType.Tile)
        { }

        public int Get(int x, int y, int outOfBoundsValue = 0)
            => !InBounds(x, y) ? outOfBoundsValue : map[mapBuffer, x, y];

        public int GetWater(int x, int y, int outOfBoundsValue = 0)
            => !InBounds(x, y) ? outOfBoundsValue : waterMap[x, y];

        public int Set(int x, int y, int value, int outOfBoundsValue = 0)
            => !InBounds(x, y) ? outOfBoundsValue : (map[mapBuffer, x, y] = value);

        public bool InBounds(int x, int y)
            => x >= 0 && x < Width
            && y >= 0 && y < Height;

        public void Randomize()
        {
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                {
                    map[0, i, j] = Random.Shared.NextDouble() < .52 ? 1 : 0;
                    map[1, i, j] = 0;
                }

            mapBuffer = 0;
        }

        public void Clear()
        {
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                {
                    map[0, i, j] = 0;
                    map[1, i, j] = 0;
                }

            mapBuffer = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (DoUpdate && (gameTime.TotalGameTime - lastUpdate).TotalMilliseconds >= 75)
            {
                lastUpdate = gameTime.TotalGameTime;

                var bornRule = new[] { 5, 6, 7, 8 };
                var stayAliveRule = new[] { 4, 5, 6, 7, 8 };

                var nextBuffer = mapBuffer == 0 ? 1 : 0;
                for (var i = 0; i < Width; i++)
                    for (var j = 0; j < Height; j++)
                    {
                        var count = CountNeighbors(i, j);
                        var value = map[mapBuffer, i, j];
                        var check = value switch
                        {
                            0 => bornRule,
                            _ => stayAliveRule
                        };

                        map[nextBuffer, i, j] = check.Contains(count) ? value + 1 : 0;
                    }

                mapBuffer = nextBuffer;

                for (var i = 0; i < Width; i++)
                    for (var j = 0; j < Height; j++)
                        waterMap[i, j] = CountNeighbors(i, j) > 0 ? 1 : 0;
            }

            BuildTileMap();
        }

        private int CountNeighbors(int x, int y)
        {
            var count = 0;
            for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    var (resolvedX, resolvedY) = ResolveCoordinates(x + i, y + j);

                    count += Get(resolvedX, resolvedY) >= 1 ? 1 : 0;
                }

            return count;
        }

        public (int x, int y) ResolveCoordinates(int x, int y)
        {
            if (x < 0)
                x = 0;

            if (y < 0)
                y = 0;

            if (x >= Width)
                x = Width - 1;

            if (y >= Height)
                y = Height - 1;

            return (x, y);
        }

        private void BuildTileMap()
        {
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                {
                    SetTile(0, j, i, GetWaterTile(i, j));
                    SetTile(1, j, i, GetGroundTile(i, j));
                }
        }

        private int GetGroundTile(int i, int j)
        {
            var val = Get(i, j) == 0 ? 0 : 1;

            var NW = Get(i - 1, j - 1, 1) == 0 ? 0 : 1;
            var N = Get(i - 1, j, 1) == 0 ? 0 : 1;
            var NE = Get(i - 1, j + 1, 1) == 0 ? 0 : 1;

            var W = Get(i, j - 1, 1) == 0 ? 0 : 1;
            var E = Get(i, j + 1, 1) == 0 ? 0 : 1;

            var SW = Get(i + 1, j - 1, 1) == 0 ? 0 : 1;
            var S = Get(i + 1, j, 1) == 0 ? 0 : 1;
            var SE = Get(i + 1, j + 1, 1) == 0 ? 0 : 1;

            var blankTiles = new[] { 0 };
            var baseTiles = new[] { 30 };
            var upperLeftTiles = new[] { 40, 41 };
            var upperTiles = new[] { 44, 45, 46, 47, 48 };
            var upperRightTiles = new[] { 42, 43 };
            var leftTiles = new[] { 60, 70, 80, 110, 120 };
            var rightTiles = new[] { 61, 71, 81, 111, 121 };
            var lowerTiles = new[] { 54, 55, 56, 57, 58 };
            var lowerRightTiles = new[] { 52, 53 };
            var lowerLeftTiles = new[] { 50, 51 };
            var lowerRightCornerTiles = new[] { 90 };
            var lowerLeftCornerTiles = new[] { 91 };
            var upperRightCornerTiles = new[] { 100 };
            var upperLeftCornerTiles = new[] { 101 };

            return MatchTiles(val, NW, N, NE, W, E, SW, S, SE,
                blankTiles, baseTiles,
                upperLeftTiles, upperTiles, upperRightTiles,
                leftTiles, rightTiles, lowerTiles, lowerRightTiles,
                lowerLeftTiles, lowerRightCornerTiles, lowerLeftCornerTiles,
                upperRightCornerTiles, upperLeftCornerTiles).First();
        }

        private static IList<int> MatchTiles(int val,
            int NW, int N, int NE,
            int W, int E,
            int SW, int S, int SE,
            int[] blankTiles, int[] baseTiles,
            int[] upperLeftTiles, int[] upperTiles, int[] upperRightTiles,
            int[] leftTiles, int[] rightTiles, int[] lowerTiles,
            int[] lowerRightTiles, int[] lowerLeftTiles, int[] lowerRightCornerTiles,
            int[] lowerLeftCornerTiles, int[] upperRightCornerTiles, int[] upperLeftCornerTiles)
        {
            return ((NW, N, NE),
                    (W, val, E),
                    (SW, S, SE)) switch
            {
                ((0, 0, 0),
                (0, 0, 0),
                (0, 0, 0)) => blankTiles,

                ((_, _, _),
                (_, 0, _),
                (_, _, _)) => blankTiles,

                ((_, 0, _),
                (0, 1, 1),
                (_, 1, _)) => upperLeftTiles,

                ((_, 0, _),
                (1, 1, 1),
                (_, 1, _)) => upperTiles,

                ((_, 0, _),
                (1, 1, 0),
                (_, 1, _)) => upperRightTiles,

                ((_, 1, _),
                (0, 1, 1),
                (_, 1, _)) => leftTiles,

                ((_, 1, _),
                (1, 1, 0),
                (_, 1, _)) => rightTiles,

                ((_, _, _),
                (1, 1, 1),
                (_, 0, _)) => lowerTiles,

                ((_, 1, _),
                (_, 1, 0),
                (_, 0, _)) => lowerRightTiles,

                ((_, _, _),
                (0, 1, _),
                (_, 0, _)) => lowerLeftTiles,

                ((0, 1, 1),
                (1, 1, 1),
                (_, 1, 1)) => lowerRightCornerTiles,

                ((1, 1, 0),
                (1, 1, 1),
                (_, 1, 1)) => lowerLeftCornerTiles,

                ((_, _, _),
                (1, 1, _),
                (0, 1, _)) => upperRightCornerTiles,

                ((1, 1, _),
                (1, 1, 1),
                (1, 1, 0)) => upperLeftCornerTiles,

                ((_, _, _),
                (_, 1, _),
                (_, _, _)) => baseTiles,

                _ => blankTiles,
            };
        }

        private int GetWaterTile(int i, int j)
        {
            var val = GetWater(i, j) == 0 ? 0 : 1;

            var NW = GetWater(i - 1, j - 1, 1) == 0 ? 0 : 1;
            var N = GetWater(i - 1, j, 1) == 0 ? 0 : 1;
            var NE = GetWater(i - 1, j + 1, 1) == 0 ? 0 : 1;

            var W = GetWater(i, j - 1, 1) == 0 ? 0 : 1;
            var E = GetWater(i, j + 1, 1) == 0 ? 0 : 1;

            var SW = GetWater(i + 1, j - 1, 1) == 0 ? 0 : 1;
            var S = GetWater(i + 1, j, 1) == 0 ? 0 : 1;
            var SE = GetWater(i + 1, j + 1, 1) == 0 ? 0 : 1;

            var blankTiles = new[] { 10 };
            var baseTiles = new[] { 12 };
            var upperLeftTiles = new[] { 1 };
            var upperTiles = new[] { 2 };
            var upperRightTiles = new[] { 3 };
            var leftTiles = new[] { 11 };
            var rightTiles = new[] { 13 };
            var lowerTiles = new[] { 22 };
            var lowerRightTiles = new[] { 23 };
            var lowerLeftTiles = new[] { 21 };
            var lowerRightCornerTiles = new[] { 15 };
            var lowerLeftCornerTiles = new[] { 14 };
            var upperRightCornerTiles = new[] { 5 };
            var upperLeftCornerTiles = new[] { 4 };

            return MatchTiles(val, NW, N, NE, W, E, SW, S, SE,
                blankTiles, baseTiles,
                upperLeftTiles, upperTiles, upperRightTiles,
                leftTiles, rightTiles, lowerTiles, lowerRightTiles,
                lowerLeftTiles, lowerRightCornerTiles, lowerLeftCornerTiles,
                upperRightCornerTiles, upperLeftCornerTiles).First();
        }
    }
}
