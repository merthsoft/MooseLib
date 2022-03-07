using Merthsoft.Moose.MooseEngine.Interface;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using Size = Roy_T.AStar.Primitives.Size;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public abstract class BaseMap : IMap
{
    public abstract int Height { get; }
    public abstract int Width { get; }
    public abstract int TileWidth { get; }
    public abstract int TileHeight { get; }
    public abstract IReadOnlyList<ILayer> Layers { get; }

    protected List<int>[,] blockingMap = new List<int>[0, 0];

    public TLayer GetLayer<TLayer>(int layerNumber) where TLayer : ILayer
        => (TLayer)Layers[layerNumber];

    public bool CellIsInBounds(Vector2 cell)
        => cell.X >= 0 && cell.X < Width
        && cell.Y >= 0 && cell.Y < Height;

    public bool CellIsInBounds(int cellX, int cellY)
        => cellX >= 0 && cellX < Width
        && cellY >= 0 && cellY < Height;

    public virtual void Update(MooseGame _game, GameTime _gameTime)
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

    public virtual IList<int> GetBlockingVector(int x, int y)
        => blockingMap[x, y];

    protected virtual Grid BaseGrid
        => Grid.CreateGridWithLateralConnections(
            new GridSize(Width, Height),
            new Size(Distance.FromMeters(1), Distance.FromMeters(1)),
            Velocity.FromMetersPerSecond(1));

    public Grid BuildCollisionGrid(params Vector2[] walkableOverrides)
        => BaseGrid.DisconnectWhere((x, y) => blockingMap[x, y].Any(t => t > 0) && !walkableOverrides.Contains(new(x, y)));

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
        catch
        {
            return Enumerable.Empty<Vector2>();
        }
    }

    public bool WorldPositionIsInBounds(float worldX, float worldY)
        => worldX > 0 && worldX < Width * TileWidth
        && worldY > 0 && worldY < Height * TileHeight;
}
