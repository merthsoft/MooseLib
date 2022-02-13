using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver;

public record SimpleTileReference<TTile>(TTile Tile, bool Blocking = false) : ITile<TTile>
{
    public bool IsBlocking(IMap map)
        => Blocking;
}
