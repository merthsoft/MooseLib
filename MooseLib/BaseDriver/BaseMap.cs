using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public abstract class BaseMap : IMap
    {
        public abstract int Height { get; }
        public abstract int Width { get; }
        public abstract int TileWidth { get; }
        public abstract int TileHeight { get; }
        public abstract IReadOnlyList<ILayer> Layers { get; }
        public abstract IEnumerable<int> ObjectLayerIndices { get; }
        public abstract IEnumerable<int> TileLayerIndices { get; }

        protected List<int>[,] blockingMap = new List<int>[0, 0];

        public bool CellIsInBounds(Vector2 cell)
            => cell.X > 0 && cell.X < Width
            && cell.Y > 0 && cell.Y < Height;

        public bool CellIsInBounds(int cellX, int cellY)
            => cellX > 0 && cellX < Width
            && cellY > 0 && cellY < Height;

        public virtual void Update(GameTime gameTime)
            => BuildFullBlockingMap();

        protected virtual void BuildFullBlockingMap()
        {
            blockingMap = new List<int>[Width, Height];

            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    blockingMap[x, y] = new();
                    for (var layerIndex = 0; layerIndex < Layers.Count; layerIndex++)
                        blockingMap[x, y].Add(IsBlockedAt(layerIndex, x, y));
                }
        }

        protected abstract int IsBlockedAt(int layer, int x, int y);

        public virtual IEnumerable<int> GetBlockingVector(int x, int y)
            => blockingMap[x, y].AsEnumerable();

        protected virtual Grid BaseGrid
            => Grid.CreateGridWithLateralConnections(
                new GridSize(Width, Height),
                new Size(Distance.FromMeters(1), Distance.FromMeters(1)),
                Velocity.FromMetersPerSecond(1));

        public Grid BuildCollisionGrid(params Vector2[] walkableOverrides)
            => BaseGrid.DisconnectWhere((x, y) => blockingMap[x, y].Any(t => t > 0) && !walkableOverrides.Contains(new(x, y)));

        public virtual IEnumerable<RayCell> FindWorldRay(Vector2 startWorldPosition, Vector2 endWorldPosition, bool fillCorners = false, bool extend = false)
        {
            var (x1, y1) = (endWorldPosition.X, endWorldPosition.Y);
            var (x2, y2) = (startWorldPosition.X, startWorldPosition.Y);

            var deltaX = (int)Math.Abs(x1 - x2);
            var deltaZ = (int)Math.Abs(y1 - y2);

            if (deltaX == 0 && deltaZ == 0)
                yield break;

            var stepX = x2 < x1 ? 1 : -1;
            var stepZ = y2 < y1 ? 1 : -1;

            var err = deltaX - deltaZ;

            RayCell BuildReturnTuple(float x, float y)
                => new(new Vector2(x, y), blockingMap[(int)(x / TileWidth), (int)(y / TileHeight)]);

            while (true)
            {
                if (!WorldPositionIsInBounds(x2, y2))
                    break;

                yield return BuildReturnTuple(x2, y2);
                if (!extend && x2 == x1 && y2 == y1)
                    break;

                var e2 = 2 * err;

                if (e2 > -deltaZ)
                {
                    err -= deltaZ;
                    x2 += stepX;
                }

                if (!WorldPositionIsInBounds(x2, y2))
                    break;

                if (fillCorners)
                    yield return BuildReturnTuple(x2, y2);

                if (!extend && x2 == x1 && y2 == y1)
                    break;

                if (e2 < deltaX)
                {
                    err += deltaX;
                    y2 += stepZ;
                }
            }
        }

        public virtual IEnumerable<Vector2> FindCellPath(Vector2 startCell, Vector2 endCell, Grid? grid = null)
        {
            if (!CellIsInBounds(startCell) || !CellIsInBounds(endCell))
                return Enumerable.Empty<Vector2>();

            grid ??= BuildCollisionGrid(startCell);

            var startX = (int)startCell.X;
            var startY = (int)startCell.Y;
            var endX = (int)endCell.X;
            var endY = (int)endCell.Y;

            try
            {
                var path = new PathFinder()
                    .FindPath(new GridPosition(startX, startY), new GridPosition(endX, endY), grid);

                if (path.Type != PathType.Complete)
                    return Enumerable.Empty<Vector2>();

                return path.Edges
                    .Select(e => new Vector2((int)e.End.Position.X, (int)e.End.Position.Y))
                    .Distinct();
            }
            catch {
                return Enumerable.Empty<Vector2>();
            }
        }

        public bool WorldPositionIsInBounds(float worldX, float worldY)
            => worldX > 0 && worldX < Width * TileWidth
            && worldY > 0 && worldY < Height * TileHeight;

        public abstract void CopyFromMap(IMap sourceMap, int sourceX = 0, int sourceY = 0, int destX = 0, int destY = 0, int? width = null, int? height = null);
    }
}
