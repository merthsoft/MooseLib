using Roy_T.AStar.Grids;

namespace Merthsoft.Moose.MooseEngine.Interface;

public interface IMap
{
    int Height { get; }
    int Width { get; }
    int TileWidth { get; }
    int TileHeight { get; }

    public Size2 TileSize => new(TileWidth, TileHeight);
    public Vector2 HalfTileSize => new(TileWidth / 2, TileHeight / 2);

    IReadOnlyList<ILayer> Layers { get; }

    void Update(MooseGame game, GameTime gameTime);
    IList<int> GetBlockingVector(int cellX, int cellY);
    IList<int> GetBlockingVector(Vector2 worldPosition)
        => GetBlockingVector((int)(worldPosition.X / TileWidth), (int)(worldPosition.Y / TileHeight));

    Grid BuildCollisionGrid(params Vector2[] walkableOverrides);
    IEnumerable<Vector2> FindCellPath(Vector2 startCell, Vector2 endCell, Grid? grid = null);

    public bool CellIsInBounds(Vector2 cell);
    public bool CellIsInBounds(int cellX, int cellY);

    public bool WorldIsInBounds(Vector2 world)
        => (int)world.X / TileWidth >= 0 && (int)world.X / TileWidth < Width
        && (int)world.Y / TileHeight >= 0 && (int)world.Y / TileHeight < Height;
}
