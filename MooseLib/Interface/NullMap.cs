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

    public Grid BuildCollisionGrid(params Vector2[] walkableOverrides)
        => Grid.CreateGridWithLateralConnections(
            new GridSize(Width, Height),
            new Size(Distance.FromMeters(1), Distance.FromMeters(1)),
            Velocity.FromMetersPerSecond(1));

    public bool CellIsInBounds(Vector2 cell)
        => false;

    public bool CellIsInBounds(int cellX, int cellY)
        => false;

    public IEnumerable<Vector2> FindCellPath(Vector2 startCell, Vector2 endCell, Grid? grid = null)
    { yield break; }

    public IEnumerable<RayCell> FindWorldRay(Vector2 startWorldPosition, Vector2 endWorldPosition, bool fillCorners = false, bool extend = false)
    { yield break; }

    public IList<int> GetBlockingVector(int cellX, int cellY) 
        => Array.Empty<int>();

    public void Update(MooseGame game, GameTime gameTime)
    { }
}
