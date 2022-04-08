using Merthsoft.Moose.MooseEngine.BaseDriver;

namespace Merthsoft.Moose.Rays;
public class RayMap : BaseMap
{
    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; } = 32;
    public override int TileHeight { get; } = 32;

    public TileLayer<int> FloorLayer { get; }
    public TileLayer<int> WallLayer { get; }
    public ObjectLayer ObjectLayer { get; }
    public TileLayer<int> CeilingLayer { get; }

    public RayMap(List<List<int>> wallMap)
    {
        Height = wallMap.Count;
        Width = wallMap[0].Count;

        FloorLayer = AddLayer(new TileLayer<int>("floor", Width, Height, -1));
        WallLayer = AddLayer(new TileLayer<int>("walls", Width, Height, -1));
        WallLayer.RendererKey = "walls";
        ObjectLayer = AddLayer(new ObjectLayer("objects"));
        CeilingLayer = AddLayer(new TileLayer<int>("ceiling", Width, Height, -1));
        
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                WallLayer.SetTileValue(x, y, wallMap[y][x]);
    }

    protected override int IsBlockedAt(string layer, int x, int y)
        => layer == "walls" ? WallLayer.GetTileValue(x, y) : 0;
}
