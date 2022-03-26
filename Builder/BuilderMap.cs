namespace Merthsoft.Moose.Builder;
public class BuilderMap : BaseMap
{
    public override int Height { get; } = 100;
    public override int Width { get; } = 100;
    public override int TileWidth { get; } = 12;
    public override int TileHeight { get; } = 12;

    public BlueprintLayer BlueprintLayer;
    public TileLayer<BuilderTiles> TileLayer;

    public BuilderMap()
    {
        BlueprintLayer = AddLayer(new BlueprintLayer(Height, Width));
        TileLayer = AddLayer(new TileLayer<BuilderTiles>("tiles", Height, Width, BuilderTiles.BrownBlock, BuilderTiles.None));
        AddLayer(new ObjectLayer("units"));
    }

    protected override int IsBlockedAt(string layer, int x, int y)
        => layer switch
        {
            "tiles" => TileLayer.GetTileValue(x, y) == BuilderTiles.None ? 0 : 1,
            _ => 0,
        };

    public void FinishBlueprint(int x, int y)
    {
        var tile = BlueprintLayer.GetTileValue(x, y);
        BlueprintLayer.SetTileValue(x, y, BuilderTiles.None);
        TileLayer.SetTileValue(x, y, tile);
        BlueprintLayer.Blueprints.RemoveAll(b => b.X == x && b.Y == y);
    }

    public BuildingBlueprintTask? AddBlueprint(BuilderTiles tile, int x, int y)
    {
        if (BlueprintLayer.GetTileValue(x, y) != BuilderTiles.None)
            return null;
        BlueprintLayer.SetTileValue(x, y, tile);
        var b = new BuildingBlueprintTask(this, x, y, 3);
        BlueprintLayer.Blueprints.Add(b);
        return b;
    }
}
