namespace Merthsoft.Moose.Rays.Actors;
public static class MissileStates
{
    public const string Flying = "Flying";
    public const string Exploding = "Exploding";
    public const string Done = "Done";
}

public record MissileDef : ActorDef
{
    public MissileDef(string missileType) : base(missileType, MissileStates.Flying) { }
}

public record RocketDef : MissileDef
{
    public RocketDef() : base("Rocket") 
    {
        States = new() {
            { MissileStates.Flying, new() { new(Blocking: false, RenderMode: ObjectRenderMode.Directional, EndAction: Missile.Move) } },
            { MissileStates.Exploding, new()
            {
                new(RenderMode: ObjectRenderMode.Sprite, Length: 100, FrameOffset: 8),
                new(RenderMode: ObjectRenderMode.Sprite, Length: 100, FrameOffset: 9),
                new(RenderMode: ObjectRenderMode.Sprite, Length: 100, FrameOffset: 10, EndAction: Missile.Explode),
                new(RenderMode: ObjectRenderMode.Sprite, Length: 100, FrameOffset: 11, EndAction: Actor.RemoveActor),
            } }
        };
    }
}

public class Missile : Actor
{
    public Vector3 MoveDirection;

    public Missile(MissileDef def, int x, int y) : base(def, x, y)
    {
        MoveDirection = Vector3.Zero;
    }

    public static void Move(Actor a)
    {
        var missile = (a as Missile)!;
        missile.Position = new(missile.Position.X + 2*missile.MoveDirection.X, missile.Position.Y +2* missile.MoveDirection.Y);
        var cell = missile.PositionIn3dSpace + 8 * missile.MoveDirection - new Vector3(8, 8, 0);
        
        if (missile.ParentMap.GetBlockingVector(new(cell.X, cell.Y)).Any(a => a > 0))
            missile.State = MissileStates.Exploding;
        
    }

    public static void Explode(Actor a)
    {
    
    }
}
