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
    IReadOnlyDictionary<string, ILayer> LayerMap { get; }

    void Update(MooseGame game, GameTime gameTime);
    IList<int> GetBlockingVector(int cellX, int cellY);
    IList<int> GetBlockingVector(Vector2 worldPosition)
        => GetBlockingVector((int)(worldPosition.X / TileWidth), (int)(worldPosition.Y / TileHeight));

    Grid BuildCollisionGrid(params Point[] walkableOverrides);
    IEnumerable<Point> FindCellPath(Point startCell, Point endCell, Grid? grid = null);

    public bool CellIsInBounds(Point cell);
    public bool CellIsInBounds(int cellX, int cellY);

    TLayer GetLayer<TLayer>(int layerNumber) where TLayer : ILayer;
    TLayer GetLayer<TLayer>(string layerName) where TLayer : ILayer;
}
