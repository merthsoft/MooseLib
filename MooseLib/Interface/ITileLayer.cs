namespace Merthsoft.MooseEngine.Interface
{
    public interface ITileLayer : ILayer
    {
        ITile GetTile(int x, int y);

        bool IsBlockedAt(int x, int y, IMap map)
            => GetTile(x, y).IsBlocking(map);
    }
}
