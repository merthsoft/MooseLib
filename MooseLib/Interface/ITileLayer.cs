namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ITileLayer : ILayer
{
    IEnumerable<Point> RendererDirtyCells { get; }
    int GetTileIndex(int x, int y);
}

public interface ITileLayer<TTile> : ITileLayer
{
    TTile GetTileValue(int x, int y);
}
