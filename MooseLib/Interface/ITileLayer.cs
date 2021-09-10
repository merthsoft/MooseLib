namespace Merthsoft.Moose.MooseEngine.Interface
{
    public interface ITileLayer<TTile> : ILayer
    {
        int Width { get; }
        int Height { get; }

        ITile<TTile> GetTile(int x, int y);
        TTile GetTileValue(int x, int y);

        bool IsBlockedAt(int x, int y, IMap map)
            => GetTile(x, y).IsBlocking(map);
    }
}
