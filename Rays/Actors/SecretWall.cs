namespace Merthsoft.Moose.Rays.Actors;

public static class SecretWallStates
{
    public const string Waiting = "Waiting";
    public const string Moving = "Moving";
    public const string Done = "Done";
}

public record SecretWallDef : ActorDef
{
    public SecretWallDef() : base("secret-wall", SecretWallStates.Waiting)
    {
        ObjectRenderMode = ObjectRenderMode.Wall;
        States = new()
        {
            { SecretWallStates.Waiting, new() { new() } },
            { SecretWallStates.Moving, new() { new(Length: 0, EndAction: SecretWall.Move) } },
            { SecretWallStates.Done, new() { new() } },
        };
    }

    public override void LoadContent(MooseContentManager contentManager) { }
}

public class SecretWall : Actor
{
    public Vector3 MoveDirection;

    public SecretWall(SecretWallDef def, int wallTile, int x, int y) : base(def, x, y)
    {
        ObjectRenderMode = ObjectRenderMode.Wall;
        TextureIndex = wallTile;
        MoveDirection = Vector3.Zero;
    }

    public override void Interact()
    {
        if (State != SecretWallStates.Waiting)
            return;

        MoveDirection = RayPlayer.Instance.FacingDirection;
        State = SecretWallStates.Moving;
    }

    public static void Move(Actor a)
    {
        var wall = (a as SecretWall)!;
        wall.Position = new(wall.Position.X + wall.MoveDirection.X, wall.Position.Y + wall.MoveDirection.Y);
        var cell = wall.PositionIn3dSpace + 16 * wall.MoveDirection - new Vector3(8, 8, 0);
        if (cell.X / 16 == (int)(cell.X / 16) && cell.Y / 16 == (int)(cell.Y / 16))
        {
            if (wall.ParentMap.GetBlockingVector(new(cell.X, cell.Y)).Any(a => a > 0))
                wall.State = SecretWallStates.Done;
        }
    }
}
