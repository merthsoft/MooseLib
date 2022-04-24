using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Rays;
public record RayGameObjectDef(string DefName, int DefaultTextureIndex, ObjectRenderMode ObjectRenderMode, int RenderBottom = 0, int RenderTop = 16) : GameObjectDef(DefName)
{
    public int DefaultTextureIndex { get; set; } = DefaultTextureIndex;
    public int RenderBottom { get; set; } = RenderBottom;
    public int RenderTop { get; set; } = RenderTop;
}

public class RayGameObject : GameObjectBase
{
    public RayGameObjectDef RayGameObjectDef;
    public int TextureIndex;
    public int TextureIndexOffset;

    public Vector3 PositionIn3dSpace => new(Position, 7);
    public Vector3 FacingDirection;

    public ObjectRenderMode ObjectRenderMode;

    public RayGameObject(RayGameObjectDef def, int x, int y) : base(def, new Vector2(x*16+8, y*16+8), layer: "objects")
    {
        RayGameObjectDef = def;
        TextureIndex = RayGameObjectDef.DefaultTextureIndex;
        FacingDirection = Vector3.Left;
        ObjectRenderMode = def.ObjectRenderMode;
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch) { }
    public override void Update(MooseGame game, GameTime gameTime) { }

    public override void PostUpdate(MooseGame game, GameTime gameTime)
    {
        FacingDirection.Normalize();
        Position.Normalize();
        base.PostUpdate(game, gameTime);
    }
}
