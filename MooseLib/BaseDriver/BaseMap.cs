using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.PathFinding.Grids;
using Merthsoft.Moose.MooseEngine.PathFinding.Paths;
using Merthsoft.Moose.MooseEngine.PathFinding.Paths.AStar;
using Merthsoft.Moose.MooseEngine.Topologies;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public abstract class BaseMap : IMap
{
    public abstract int Height { get; }
    public abstract int Width { get; }
    public abstract int TileWidth { get; }
    public abstract int TileHeight { get; }
    public Topology Topology { get; set; } = Topology.Plane;
    
    protected List<ILayer> layers = new();
    public virtual IReadOnlyList<ILayer> Layers => layers.AsReadOnly();

    protected Dictionary<string, ILayer> layerMap = new();
    public virtual IReadOnlyDictionary<string, ILayer> LayerMap => layerMap.AsReadOnly();

    protected List<int>[,] blockingMap = null!;
    protected Grid blockingGrid = null!;
    public Grid BlockingGrid => blockingGrid;

    private IPathFinder PathFinder = new AStarPathFinder();

    public TLayer AddLayer<TLayer>(TLayer layer) where TLayer : ILayer
    {
        layers.Add(layer);
        layerMap[layer.Name] = layer;

        return layer;
    }

    public void ClearLayers()
    {
        layers.Clear();
        layerMap.Clear();
    }

    public TLayer GetLayer<TLayer>(int layerNumber) where TLayer : ILayer
        => (TLayer)Layers[layerNumber];

    public TLayer GetLayer<TLayer>(string layerName) where TLayer : ILayer
        => (TLayer)Layers.First(l => l.Name == layerName);

    public Point TranslatePoint(int x, int y)
        => TopologyHelper.TranslatePoint(x, y, Topology, Width, Height);

    public Point TranslatePoint(Point cell)
        => cell.TranslatePoint(Topology, Width, Height);

    public Vector2 TranslateVector(Vector2 cell)
        => cell.TranslateVector(Topology, Width, Height);

    public bool CellIsInBounds(Point cell)
    {
        cell = cell.TranslatePoint(Topology, Width, Height);
        return cell.X >= 0 && cell.X < Width
            && cell.Y >= 0 && cell.Y < Height;
    }

    public bool PositionIsInBounds(Vector2 position)
    {
        position = position.TranslateVector(Topology, Width, Height);
        return position.X >= 0 && position.X < Width * TileWidth
            && position.Y >= 0 && position.Y < Height * TileHeight;
    }

    public bool CellIsInBounds(int cellX, int cellY)
        => CellIsInBounds(new(cellX, cellY));

    public virtual void Update(MooseGame game, GameTime gameTime)
        => BuildFullBlockingMap();

    protected virtual void BuildFullBlockingMap()
    {
        if (blockingMap == null)
            blockingMap = new List<int>[Width, Height];
        if (blockingGrid == null)
            blockingGrid = BaseGrid();

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                blockingMap[x, y] ??= new();
                blockingMap[x, y].Clear();
                foreach (var layer in layers)
                {
                    var blocked = IsBlockedAt(layer.Name, x, y);
                    blockingMap[x, y].Add(blocked);
                    if (blocked > 0)
                        blockingGrid.DisconnectIncoming(x, y);
                    else
                        blockingGrid.ConnectIncomingLaterally(x, y, DefaultVelocity);
                }
            }
    }

    public void MoveObject(GameObjectBase gameObject, Vector2 toPosition)
    {
        var layer = gameObject.Layer;
        var layerIndex = layers.IndexOf(l => l.Name == layer);
        var (x, y) = gameObject.Cell;
        gameObject.Position = toPosition;
        FixGrid(x, y);

        (x, y) = gameObject.Cell;
        FixGrid(x, y);

        void FixGrid(int x, int y)
        {
            return;
            var blockedAt = IsBlockedAt(layer, x, y);
            blockingMap[x, y][layerIndex] = blockedAt;

            if (blockedAt > 0)
                blockingGrid.DisconnectNode(x, y);
            else
                blockingGrid.ConnectIncomingLaterally(x, y, DefaultVelocity);
        }
    }

    public abstract int IsBlockedAt(string layer, int x, int y);

    public virtual IList<int> GetBlockingVector(int x, int y)
    {
        (x, y) = TopologyHelper.TranslatePoint(x, y, Topology, Width, Height);
        return x < 0 || x >= Width || y < 0 || y >= Height ? new() { } : blockingMap[x, y];
    }

    protected Merthsoft.Moose.MooseEngine.PathFinding.Primitives.Velocity DefaultVelocity
        => Merthsoft.Moose.MooseEngine.PathFinding.Primitives.Velocity.FromMetersPerSecond(1);

    protected Merthsoft.Moose.MooseEngine.PathFinding.Primitives.Distance DefaultDistance
        => Merthsoft.Moose.MooseEngine.PathFinding.Primitives.Distance.FromMeters(1);

    protected virtual Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid BaseGrid()
        => Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid.CreateGridWithLateralConnections(
            new Merthsoft.Moose.MooseEngine.PathFinding.Primitives.GridSize(Width, Height),
            new Merthsoft.Moose.MooseEngine.PathFinding.Primitives.Size(DefaultDistance, DefaultDistance),
            DefaultVelocity);

    public Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid BuildCollisionGrid(params Point[] walkableOverrides)
        => BaseGrid().DisconnectIncomingWhere((x, y) => blockingMap[x, y].Any(t => t > 0) && !walkableOverrides.Contains(new(x, y)));

    public virtual IEnumerable<Point> FindCellPath(Point startCell, Point endCell, Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid? grid = null)
    {
        if (!CellIsInBounds(startCell) || !CellIsInBounds(endCell))
            return Enumerable.Empty<Point>();

        var startX = startCell.X;
        var startY = startCell.Y;
        var endX = endCell.X;
        var endY = endCell.Y;

        try
        {
            var g = grid ?? blockingGrid;
            //if (grid == null)
            //    g.ConnectOutgoing(startX, endX, DefaultVelocity);

            var path = PathFinder.FindPath(startX, startY, endX, endY, g);

            //var blocked = blockingMap[startX, startY].Any(x => x > 0);
            //if (blocked)
            //    blockingGrid.DisconnectNode(startX, startY);

            //if (path.Type != PathType.Complete)
            //    return Enumerable.Empty<Point>();

            if (path.Edges.Count == 0)
                return Enumerable.Empty<Point>();

            return path.Edges
                .Select(e => e.End.PointPosition)
                .Distinct();
        }
        catch
        {
            return Enumerable.Empty<Point>();
        }
    }
}
