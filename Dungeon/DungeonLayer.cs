using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;
internal class DungeonLayer : TileLayer<Tile>, ITileLayer<int>
{
    public DungeonLayer(string name, int width, int height) : base(name, width, height, Tile.None)
    {

    }

    int ITileLayer<int>.GetTileValue(int x, int y) => (int)GetTileValue(x, y);
}
