namespace Merthsoft.Moose.Rays;

public record DoorDef : RayGameObjectDef { public DoorDef() : base("door", 57, ObjectRenderMode.Door) { } }

public class Door : RayGameObject
{
    public Door(RayGameObjectDef def, int x, int y, bool horizontal) : base(def, x, y)
    {

    }
}
