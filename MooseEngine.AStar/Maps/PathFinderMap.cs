using Merthsoft.Moose.MooseEngine.BaseDriver;

namespace Merthsoft.Moose.MooseEngine.PathFinding.Maps;
public class PathFinderMap : BaseMap
{
    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; }
    public override int TileHeight { get; }

    public IPathFinder PathFinder;
    protected Grid blockingGrid = null!;
    public Grid BlockingGrid => blockingGrid;

    public PathFinderMap(IPathFinder pathFinder)
        => PathFinder = pathFinder;

    public override int IsBlockedAt(string layer, int x, int y) => throw new NotImplementedException();

    protected override void BuildFullBlockingMap()
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

    protected Velocity DefaultVelocity
        => Velocity.FromMetersPerSecond(1);

    protected Distance DefaultDistance
        => Distance.FromMeters(1);

    protected virtual Grid BaseGrid()
        => Grid.CreateGridWithLateralConnections(
            new GridSize(Width, Height),
            new Size(DefaultDistance, DefaultDistance),
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
