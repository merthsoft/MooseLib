namespace Merthsoft.Moose.Rays;

public record TreasureDef(int Value, string DefName, int DefaultTextureIndex) : RayGameObjectDef(DefName, DefaultTextureIndex, ObjectRenderMode.Sprite, RenderBottom: 8) { }

public class Treasure : RayGameObject {
    public Treasure(RayGameObjectDef def, int x, int y) : base(def, x, y)
    {

    }
}