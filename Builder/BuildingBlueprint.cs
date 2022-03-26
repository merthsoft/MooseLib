namespace Merthsoft.Moose.Builder;
public class BuildingBlueprintTask : BuilderTask
{
    BuilderMap Map;
    public override string Name => "Build";

    public BuildingBlueprintTask(BuilderMap map, int x, int y, int timeRequired) : base(x, y, timeRequired)
    {
        Map = map;
    }

    public override void Complete()
    {
        base.Complete();
        Map.FinishBlueprint(X, Y);
    }
}
