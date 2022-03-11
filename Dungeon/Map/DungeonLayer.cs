using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Map;
public class DungeonLayer : TileLayer<DungeonTile>, ITileLayer<int>
{
    public DungeonLayer(int width, int height) : base("dungeon", width, height, DungeonTile.BrickWall, DungeonTile.None)
    {

    }

    int ITileLayer<int>.GetTileValue(int x, int y) => (int)GetTileValue(x, y);
}
