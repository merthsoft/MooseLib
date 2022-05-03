namespace Merthsoft.Moose.Rays.Actors;

public static class DoorStates
{
    public const string Open = "Open";
    public const string Closed = "Closed";
    public const string Opening = "Opening";
    public const string Closing = "Closing";
}

public record DoorDef : ActorDef
{
    public DoorDef() : base("door", DefaultState: DoorStates.Closed)
    {
        DefaultTextureIndex = 1;
        ObjectRenderMode = ObjectRenderMode.Door;
        States = new()
        {
            { DoorStates.Open, new() { new(Blocking: false, Length: 2000, EndAction: Door.CheckClose) } },
            { DoorStates.Closed, new() { new(Blocking: true) } },
            { DoorStates.Opening, new() { new(Length: 25, EndAction: Door.Opening) } },
            { DoorStates.Closing, new() { new(Length: 25, EndAction: Door.Closing) } }
        };
    }
}

public class Door : Actor
{
    public float OpenPercent;
    public bool Horizontal;

    public Door(DoorDef def, int x, int y, bool horizontal, int doorTile) : base(def, x, y)
    {
        Horizontal = horizontal;
        TextureIndex = doorTile;
    }

    public override void Interact()
    {
        State = State switch
        {
            DoorStates.Closed => DoorStates.Opening,
            DoorStates.Closing => DoorStates.Opening,
            DoorStates.Open => DoorStates.Closing,
            _ => DoorStates.Closing
        };
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

    public static void CheckClose(Actor a)
    {

    }
}
