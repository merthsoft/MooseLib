namespace Merthsoft.Moose.Rays;

public record WeaponDef(string DefName, int DefaultTextureIndex, int TextureRow, Keys Key, int NumFrames, List<int> AttackFrames) 
    : RayGameObjectDef(DefName, DefaultTextureIndex, ObjectRenderMode.Sprite, 8)
{ }

public class Weapon : RayGameObject
{
    WeaponDef WeaponDef;
    public Weapon(WeaponDef def, int x, int y) : base(def, x, y)
    {
        WeaponDef = def;
    }
}