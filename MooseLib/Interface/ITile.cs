namespace Merthsoft.Moose.MooseEngine.Interface
{
    public interface ITile<T>
    {
        T Tile { get; }
        bool IsBlocking(IMap map);
    }
}
