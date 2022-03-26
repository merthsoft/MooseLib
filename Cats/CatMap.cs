namespace Merthsoft.Moose.Cats;
public class CatMap : BaseMap
{
    public override int Height { get; } = 100;
    public override int Width { get; } = 100;
    public override int TileWidth { get; } = 32;
    public override int TileHeight { get; } = 32;

    public CatMap()
    {
        AddLayer(new ObjectLayer("cats"));
    }

    protected override int IsBlockedAt(string layer, int x, int y) => 0;
}
