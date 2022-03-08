using Roy_T.AStar.Grids;
using Roy_T.AStar.Primitives;
using Size = Roy_T.AStar.Primitives.Size;

namespace Merthsoft.Moose.MooseEngine.Interface;

public class NullMap : IMap
{
    public static NullMap Instance = new();

    public int Height => 0;
    public int Width => 0;
    public int TileWidth => 0;
    public int TileHeight => 0;
    public IReadOnlyList<ILayer> Layers { get; } = new List<ILayer>().AsReadOnly();

    public Grid BuildCollisionGrid(params Point[] walkableOverrides)
        => Grid.CreateGridWithLateralConnections(
            new GridSize(Width, Height),
            new Size(Distance.FromMeters(1), Distance.FromMeters(1)),
            Velocity.FromMetersPerSecond(1));

    public bool CellIsInBounds(Point cell)
        => false;

    public bool CellIsInBounds(int cellX, int cellY)
        => false;

    public IEnumerable<Point> FindCellPath(Point startCell, Point endCell, Grid? grid = null)
    { yield break; }

    public IList<int> GetBlockingVector(int cellX, int cellY) 
        => Array.Empty<int>();

    public void Update(MooseGame game, GameTime gameTime)
    { }
}
