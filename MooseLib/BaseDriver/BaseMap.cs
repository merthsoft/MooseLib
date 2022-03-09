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
    
    protected List<ILayer> layers = new();
    public virtual IReadOnlyList<ILayer> Layers => layers.AsReadOnly();

    protected Dictionary<string, ILayer> layerMap = new();
    public virtual IReadOnlyDictionary<string, ILayer> LayerMap => layerMap.AsReadOnly();


    protected List<int>[,] blockingMap = new List<int>[0, 0];

    public TLayer AddLayer<TLayer>(TLayer layer) where TLayer : ILayer
    {
        layers.Add(layer);
        layerMap[layer.Name] = layer;

        return layer;
    }

    public TLayer GetLayer<TLayer>(int layerNumber) where TLayer : ILayer
        => (TLayer)Layers[layerNumber];

    public TLayer GetLayer<TLayer>(string layerName) where TLayer : ILayer
        => (TLayer)Layers.First(l => l.Name == layerName);

    public bool CellIsInBounds(Point cell)
        => cell.X >= 0 && cell.X < Width
        && cell.Y >= 0 && cell.Y < Height;

    public bool PositionIsInBounds(Vector2 position)
        => position.X >= 0 && position.X < Width * TileWidth
        && position.Y >= 0 && position.Y < Height * TileHeight;

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
                foreach (var layer in layers)
                    blockingMap[x, y].Add(IsBlockedAt(layer.Name, x, y));
            }
    }

    protected abstract int IsBlockedAt(string layer, int x, int y);

    public virtual IList<int> GetBlockingVector(int x, int y)
        => blockingMap[x, y];

    protected virtual Grid BaseGrid
        => Grid.CreateGridWithLateralConnections(
            new GridSize(Width, Height),
            new Size(Distance.FromMeters(1), Distance.FromMeters(1)),
            Velocity.FromMetersPerSecond(1));

    public Grid BuildCollisionGrid(params Point[] walkableOverrides)
        => BaseGrid.DisconnectWhere((x, y) => blockingMap[x, y].Any(t => t > 0) && !walkableOverrides.Contains(new(x, y)));

    public virtual IEnumerable<Point> FindCellPath(Point startCell, Point endCell, Grid? grid = null)
    {
        if (!CellIsInBounds(startCell) || !CellIsInBounds(endCell))
            return Enumerable.Empty<Point>();

        grid ??= BuildCollisionGrid(startCell);

        var startX = startCell.X;
        var startY = startCell.Y;
        var endX = endCell.X;
        var endY = endCell.Y;

        try
        {
            var path = new PathFinder()
                .FindPath(new GridPosition(startX, startY), new GridPosition(endX, endY), grid);

            if (path.Type != PathType.Complete)
                return Enumerable.Empty<Point>();

            return path.Edges
                .Select(e => new Point((int)e.End.Position.X, (int)e.End.Position.Y))
                .Distinct();
        }
        catch
        {
            return Enumerable.Empty<Point>();
        }
    }

    public bool WorldPositionIsInBounds(float worldX, float worldY)
        => worldX > 0 && worldX < Width * TileWidth
        && worldY > 0 && worldY < Height * TileHeight;
}
