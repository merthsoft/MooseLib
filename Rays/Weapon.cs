using Merthsoft.Moose.Rays.Actors;

namespace Merthsoft.Moose.Rays;

public record WeaponDef(string DefName, int DefaultTextureIndex, int TextureRow, Keys Key, int NumFrames, List<int> AttackFrames, Action<RayGameObject> Attack)
    : RayGameObjectDef(DefName, DefaultTextureIndex, ObjectRenderMode.Sprite, 8)
{
}

public class Weapon : RayGameObject
{
    WeaponDef WeaponDef;
    public Weapon(WeaponDef def, int x, int y) : base(def, x, y)
    {
        WeaponDef = def;
    }

    public static void RayAttack(RayGameObject a)
    {
        var enemy = a.VisibleObjects().OfType<Actor>().FirstOrDefault(a => a.Shootable);
        enemy?.TakeDamage(1);
    }

    public static void Rocket(RayGameObject a)
    {
        var cell = (a.PositionIn3dSpace + 16 * a.FacingDirection) / 16;
        RayGame.Instance.SpawnRocket((int)cell.X, (int)cell.Y, a.FacingDirection);
    }
}