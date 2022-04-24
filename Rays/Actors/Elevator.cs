namespace Merthsoft.Moose.Rays.Actors;

public static class ElevatorStates
{
    public const string Waiting = "Waiting";
    public const string Moving = "Switched";
}

public record ElevatorDef : ActorDef
{
    public ElevatorDef() : base("elevator", SecretWallStates.Waiting)
    {
        ObjectRenderMode = ObjectRenderMode.Wall;
        States = new()
        {
            { ElevatorStates.Waiting, new() { new() } }
        };
    }

    public override void LoadContent(MooseContentManager contentManager) { }
}

public class Elevator : Actor
{
    public Elevator(ElevatorDef def, bool up, int x, int y) : base(def, x, y)
    {
        ObjectRenderMode = ObjectRenderMode.Wall;
        TextureIndex = up ? 16 : 17;
    }
}
