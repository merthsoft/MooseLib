using Merthsoft.Moose.MooseEngine.BaseDriver;
using System.Linq;

namespace Merthsoft.Moose.Rts;
internal class RtsMap : BaseMap
{
    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; } = 8;
    public override int TileHeight { get; } = 8;

    public ObjectLayer<Unit> UnitLayer { get; }
    protected int[,] reservedMap = null!;
    public int[,] ReservationMap => reservedMap;

    public RtsMap(int width, int height) : base()
    {
        Height = height;
        Width = width;
        UnitLayer = AddLayer(new ObjectLayer<Unit>("units", width, height));
        reservedMap = new int[width, height];
    }

    public override int IsBlockedAt(string layer, int x, int y)
        => reservedMap[x, y]; // UnitLayer.GetObjects(x, y).Any() ? 1 : reservedMap[x, y];

    public void ReserveLocation(string layer, Point position)
    {
        var layerIndex = layers.IndexOf(l => l.Name == layer);
        var (x, y) = position;
        reservedMap[x, y] = 1;
        blockingMap[x, y][layerIndex] = IsBlockedAt(layer, x, y);
    }

    public void ClearReservation(string layer, Point position)
    {
        var layerIndex = layers.IndexOf(l => l.Name == layer);
        var (x, y) = position;
        reservedMap[x, y] = 0;
        blockingMap[x, y][layerIndex] = IsBlockedAt(layer, x, y);
    }
}
