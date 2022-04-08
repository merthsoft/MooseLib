namespace Merthsoft.Moose.Rays.Intersections;

public record Intersection(Vector2 Position, int WallNumber, bool NorthWall)
{
    public RayTexture GetTexture()
        => NorthWall
            ? RayGame.Instance.DarkWalls[WallNumber]
            : RayGame.Instance.Walls[WallNumber];
}
