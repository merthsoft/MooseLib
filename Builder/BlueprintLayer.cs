namespace Merthsoft.Moose.Builder;
public class BlueprintLayer : TileLayer<BuilderTiles>
{
    public List<BuildingBlueprintTask> Blueprints = new();

    public BlueprintLayer(int width, int height) : base("blueprints", width, height, BuilderTiles.None, BuilderTiles.None)
    {
        DrawColor = Color.DarkGray.HalveAlphaChannel();
    }
}
