using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.BaseDriver;

namespace Merthsoft.Moose.Dungeon.Map;
public class DungeonLayer : TileLayer<DungeonTile>
{
    public DungeonLayer(int width, int height) : base("dungeon", width, height, DungeonTile.BrickWall, DungeonTile.None)
    {

    }
}
