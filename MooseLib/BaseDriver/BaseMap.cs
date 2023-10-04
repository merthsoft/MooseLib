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
    
    public int NumLayers { get; set; }
    protected List<ILayer> layers = new();
    public virtual IReadOnlyList<ILayer> Layers => layers.AsReadOnly();

    protected Dictionary<string, ILayer> layerMap = new();
    public virtual IReadOnlyDictionary<string, ILayer> LayerMap => layerMap.AsReadOnly();

    public abstract int IsBlockedAt(string layer, int x, int y);

    public virtual IEnumerable<int> GetBlockingVector(int x, int y)
    {
        (x, y) = TopologyHelper.TranslatePoint(x, y, Topology, Width, Height);
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return Enumerable.Empty<int>();
        
        return Layers.Select(l => IsBlockedAt(l.Name, x, y));
    }

    public TLayer AddLayer<TLayer>(TLayer layer) where TLayer : ILayer
    {
        layers.Add(layer);
        layerMap[layer.Name] = layer;
        NumLayers++;
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
        => CellIsInBounds(cell.X, cell.Y);

    public bool PositionIsInBounds(Vector2 position)
    {
        position = position.TranslateVector(Topology, Width, Height);
        return position.X >= 0 && position.X < Width * TileWidth
            && position.Y >= 0 && position.Y < Height * TileHeight;
    }

    public bool CellIsInBounds(int cellX, int cellY)
    {
        var cell = TranslatePoint(cellX, cellY);
        return cell.X >= 0 && cell.X < Width
            && cell.Y >= 0 && cell.Y < Height;
    }

    public abstract void Update(MooseGame game, GameTime gameTime);
}
