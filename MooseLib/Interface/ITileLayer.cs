namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ITileLayer<TTile> : ILayer
{
    int Width { get; }
    int Height { get; }

    TTile GetTileValue(int x, int y);
}
