using Merthsoft.Moose.MooseEngine.BaseDriver;

namespace Merthsoft.Moose.Rays;
public class RayMap : BaseMap
{
    private int height;
    public override int Height => height;

    private int width;
    public override int Width => width;

    public override int TileWidth { get; } = 16;
    public override int TileHeight { get; } = 16;

    public TileLayer<int> FloorLayer { get; set; }   = null!;
    public TileLayer<int> WallLayer { get; set; }    = null!;
    public ObjectLayer ObjectLayer { get; set; }     = null!;
    public TileLayer<int> CeilingLayer { get; set; } = null!;

    public RayMap()
    {
        height = 64;
        width = 64;

        FloorLayer = AddLayer(new TileLayer<int>("floor", Width, Height, -1, 54));
        WallLayer = AddLayer(new TileLayer<int>("walls", Width, Height, 1));
        WallLayer.RendererKey = "walls";
        ObjectLayer = AddLayer(new ObjectLayer("objects"));
        CeilingLayer = AddLayer(new TileLayer<int>("ceiling", Width, Height, -1, 0));
    }

    public void InitializeWalls(List<List<int>> wallMap)
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                WallLayer.SetTileValue(x, y, wallMap[y][x]);
    }

    protected override int IsBlockedAt(string layer, int x, int y)
        => layer == "walls" ? WallLayer.GetTileValue(x, y) + 1 : 0;
}
