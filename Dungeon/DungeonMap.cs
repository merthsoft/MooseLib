using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;
internal class DungeonMap : BaseMap
{
    private readonly DungeonLayer dungeonLayer;

    public override int Height { get; }
    public override int Width { get; }
    public override int TileWidth { get; }
    public override int TileHeight { get; }
    public override IReadOnlyList<ILayer> Layers { get; }

    public DungeonMap()
    {
        Layers = new ILayer[]
        {
            dungeonLayer = new DungeonLayer("dungeon", 100, 100),
            new ObjectLayer("player"),
        };
    }

    protected override int IsBlockedAt(int layer, int x, int y) 
        => dungeonLayer.GetTileValue(x, y) > Tile.BLOCKING_START ? 1 : 0;
}
