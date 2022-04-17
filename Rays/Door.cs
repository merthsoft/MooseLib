namespace Merthsoft.Moose.Rays;

public static class DoorStates
{
    public const string Open = "Open";
    public const string Closed = "Closed";
    public const string Opening = "Opening";
    public const string Closing = "Closing";
}

public record DoorDef : ActorDef { 
    public DoorDef() : base("door", 48, DefaultState: DoorStates.Closed) {
        RenderMode = ObjectRenderMode.Door;
        States = new()
        {
            { DoorStates.Open, new(Length: 1000, NextState: DoorStates.Closing) },
            { DoorStates.Closed,new(Length: 1000, NextState: DoorStates.Opening) },
            { DoorStates.Opening, new(Length: 100, EndAction: Door.Opening) },
            { DoorStates.Closing, new(Length: 100, EndAction: Door.Closing) }
        };
    } 
}

public class Door : Actor
{
    public float OpenPercent;
    public bool Horizontal;

    public Door(DoorDef def, int x, int y, bool horizontal) : base(def, x, y)
    {
        Horizontal = horizontal;
    }

    public static void Opening(Actor a)
    {
        var door = (a as Door)!;
        door.OpenPercent += .1f;
        if (door.OpenPercent >= 1)
        {
            door.OpenPercent = 1;
            door.State = DoorStates.Open;
        }
    }

    public static void Closing(Actor a)
    {
        var door = (a as Door)!;
        door.OpenPercent -= .1f;
        if (door.OpenPercent <= 0)
        {
            door.OpenPercent = 0;
            door.State = DoorStates.Closed;
        }
    }
}
