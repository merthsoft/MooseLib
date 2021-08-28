namespace Merthsoft.MooseEngine.Interface
{
    public interface ITile<T>
    {
        T Tile { get; }
        bool IsBlocking(IMap map);
    }
}
