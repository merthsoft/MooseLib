using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;
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

    public abstract int IsBlockedAt(string layer, int x, int y);

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
        => (TLayer)layers[layerNumber];

    public TLayer GetLayer<TLayer>(string layerName) where TLayer : ILayer
        => (TLayer)layerMap[layerName];

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

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                blockingMap[x, y] ??= new();
                blockingMap[x, y].Clear();
                foreach (var layer in layers)
                {
                    var blocked = IsBlockedAt(layer.Name, x, y);
                    blockingMap[x, y].Add(blocked);
                }
            }
    }

    public virtual IList<int> GetBlockingVector(int x, int y)
    {
        (x, y) = TopologyHelper.TranslatePoint(x, y, Topology, Width, Height);
        return x < 0 || x >= Width || y < 0 || y >= Height ? new() { } : blockingMap[x, y];
    }

    public IList<int> GetBlockingVector(Vector2 worldPosition)
        => GetBlockingVector((int)(worldPosition.X / TileWidth), (int)(worldPosition.Y / TileHeight));
}
