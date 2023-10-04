using Merthsoft.Moose.MooseEngine.BaseDriver;

namespace Merthsoft.Moose.MooseEngine.PathFinding.Maps;

public class PathFinderMap : BaseMap
{
    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; }
    public override int TileHeight { get; }

    protected int[,][] blockingMap = null!;
    public IPathFinder PathFinder;
    protected Grid blockingGrid = null!;
    public Grid BlockingGrid => blockingGrid;
    
    private bool isBlockingMapDirty = true;
    public bool IsBlockingMapDirty
    {
        get => isBlockingMapDirty;
        set
        {
            isBlockingMapDirty = value;
            if (isBlockingMapDirty)
                PathFinder.ClearCache();
        }
    }
    
    public PathFinderMap(IPathFinder pathFinder)
        => PathFinder = pathFinder;

    public override int IsBlockedAt(string layer, int x, int y) 
        => throw new NotImplementedException();

    public override void Update(MooseGame game, GameTime gameTime)
        => BuildFullBlockingMap();


    protected void BuildFullBlockingMap()
    {
        if (!IsBlockingMapDirty)
            return;
        IsBlockingMapDirty = false;

        if (blockingMap == null)
            blockingMap = new int[Width, Height][];
        if (blockingGrid == null)
            blockingGrid = BaseGrid();

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                blockingMap[x, y] ??= new int[Layers.Count];

                var overall = 0;
                for (var i = 0; i < layers.Count; i++)
                {
                    var blocked = IsBlockedAt(layers[i].Name, x, y);
                    overall += blocked;
                    if (blockingMap[x, y][i] == blocked)
                        continue;
                    
                    blockingMap[x, y][i] = blocked;
                }

                if (overall > 0)
                    blockingGrid.DisconnectIncoming(x, y);
                else
                    blockingGrid.ReconnectIncoming(x, y);
            }
        
    }

    protected Velocity DefaultVelocity
        => Velocity.FromMetersPerSecond(1);

    protected Distance DefaultDistance
        => Distance.FromMeters(1);

    protected virtual Grid BaseGrid()
        => Grid.CreateGridWithLateralAndDiagonalConnections(
            new Size(Width, Height),
            new CellSize(DefaultDistance, DefaultDistance),
            DefaultVelocity);

    public Grid BuildCollisionGrid(params Point[] walkableOverrides)
        => BaseGrid().DisconnectIncomingWhere((x, y) => blockingMap[x, y].Any(t => t > 0) && !walkableOverrides.Contains(new(x, y)));

    public virtual IEnumerable<Point> FindCellPath(Point startCell, Point endCell, Grid? grid = null, IPathFinder? pathFinder = null)
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

            var path = (pathFinder ?? PathFinder).FindPath(startX, startY, endX, endY, g);

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
