using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.PathFinding.Maps;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
using System.Collections.Generic;

namespace Merthsoft.Moose.Rts;
internal class RtsMap : PathFinderMap
{
    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; } = 8;
    public override int TileHeight { get; } = 8;

    public TileLayer<int> TileLayer { get; }
    public ObjectLayer<Unit> UnitLayer { get; }
    public int[,] ReservationMap => TileLayer.Tiles;

    public RtsMap(int width, int height) : base(new FlowFieldPathFinder())
    {
        Height = height;
        Width = width;
        TileLayer = AddLayer(new TileLayer<int>("tiles", Width, Height, -1));
        UnitLayer = AddLayer(new ObjectLayer<Unit>("units", width, height));
    }

    public IEnumerable<Unit> GetUnits()
        => UnitLayer.Objects;

    public override int IsBlockedAt(string layer, int x, int y)
        => TileLayer.GetTileValue(x, y); // UnitLayer.GetObjects(x, y).Any() ? 1 : reservedMap[x, y];

    public bool IsCellInBounds(int x, int y)
        => CellIsInBounds(x, y);

    public void SetReservationIfInBounds(int x, int y, int value)
    {
        if (IsCellInBounds(x, y))
            TileLayer.SetTileValue(x, y, value);

    }
}
