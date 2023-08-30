using Merthsoft.Moose.Rays.Actors;

namespace Merthsoft.Moose.Rays;

public record WeaponDef(string DefName, int DefaultTextureIndex, int TextureRow, Keys Key, int NumFrames, List<int> AttackFrames, Action<RayGameObject> Attack)
    : RayGameObjectDef(DefName, DefaultTextureIndex, ObjectRenderMode.Sprite, false)
{
}

public class Weapon : RayGameObject
{
    WeaponDef WeaponDef;

    public Weapon(WeaponDef def, int x, int y) : base(def, x, y)
    {
        WeaponDef = def;
    }

    public static void KnifeAttack(RayGameObject o)
    {
        var checkCell3 = o.PositionIn3dSpace + 16 * o.FacingDirection;
        var checkCell = new Point((int)(checkCell3.X / 16), (int)(checkCell3.Y / 16));
        var actor = RayGame.Instance.ReadObjects.OfType<Actor>().FirstOrDefault(a => a.Shootable && a.Cell == checkCell);
        actor?.TakeDamage(1);
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