using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public record SimpleTileContainer<T>(T Value, bool Blocking = false) : ITile
    {
        public bool IsBlocking(IMap map)
            => Blocking;
    }
}
