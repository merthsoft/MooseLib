using MonoGame.Extended.Tiled;

namespace Merthsoft.Moose.MageQuest;
public class MageMap : TiledMooseMap
{
    public override int Height { get; } = 50;
    public override int Width { get; } = 50;
    public override int TileWidth { get; } = 16;
    public override int TileHeight { get; } = 16;

    public MageMap(TiledMap map) : base(map)
    {
        
    }
}
