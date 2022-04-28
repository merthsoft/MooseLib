namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ITileLayer<TTile> : ILayer
{
    TTile GetTileValue(int x, int y);
}
