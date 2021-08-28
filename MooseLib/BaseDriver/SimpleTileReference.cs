using Merthsoft.MooseEngine.Interface;

namespace Merthsoft.MooseEngine.BaseDriver
{
    public record SimpleTileReference<TTile>(TTile Tile, bool Blocking = false) : ITile<TTile>
    {
        public bool IsBlocking(IMap map)
            => Blocking;
    }
}
