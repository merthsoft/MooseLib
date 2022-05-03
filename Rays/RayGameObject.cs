using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.Rays.Serialization;

namespace Merthsoft.Moose.Rays;
public record RayGameObjectDef(string DefName, int DefaultTextureIndex,
    ObjectRenderMode ObjectRenderMode, bool Blocking,
    int RenderBottom = 0, int RenderTop = 16,
    List<Frame>? Frames = null) : GameObjectDef(DefName)
{
    public int DefaultTextureIndex { get; set; } = DefaultTextureIndex;

    public RayGameObjectDef(ObjectDefinition definition)
        : this(definition.Name, definition.FirstFrameIndex,
             definition.Type == ObjectType.Overlay ? ObjectRenderMode.Overlay : ObjectRenderMode.Sprite, 
             definition.Blocking, Frames: definition.Frames)
    { }
}

public class RayGameObject : GameObjectBase
{
    public RayGameObjectDef RayGameObjectDef;
    public int TextureIndex;
    public int TextureIndexOffset;

    public Vector3 PositionIn3dSpace => new(Position, 7);
    public Vector3 FacingDirection;

    public ObjectRenderMode ObjectRenderMode;

    public virtual bool Blocking => RayGameObjectDef.Blocking;

    public int FrameIndex;
    public double FrameCounter;
    public double FrameTimer;
    
    public RayGameObject(RayGameObjectDef def, int x, int y) : base(def, new Vector2(x*16+8, y*16+8), layer: "objects")
    {
        RayGameObjectDef = def;
        TextureIndex = RayGameObjectDef.DefaultTextureIndex;
        FacingDirection = Vector3.Left;
        ObjectRenderMode = def.ObjectRenderMode;
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch) { }
    public override void Update(MooseGame game, GameTime gameTime)
    {
        var frames = RayGameObjectDef.Frames;
        if (frames != null && frames.Count > 1)
        {
            FrameCounter += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (FrameCounter >= FrameTimer)
            {
                FrameIndex++;
                if (FrameIndex >= frames.Count)
                    FrameIndex = 0;
                FrameTimer = game.Random.Next(frames[FrameIndex].MinTime, frames[FrameIndex].MaxTime);
                FrameCounter = 0;
            }
            TextureIndex = frames[FrameIndex].Index;
        }
    }

    public override void PostUpdate(MooseGame game, GameTime gameTime)
    {
        FacingDirection.Normalize();
        Position.Normalize();
        base.PostUpdate(game, gameTime);
    }

    public IEnumerable<RayGameObject> VisibleObjects()
    {
        var parentMap = (ParentMap as RayMap)!;

        var playerRotation = -MathF.Atan2(FacingDirection.Y, FacingDirection.X);
        var matrix = Matrix.CreateRotationZ(playerRotation);
        var fov = MathHelper.ToRadians(50);

        foreach (var obj in RayGame.Instance.ReadObjects.Cast<RayGameObject>().OrderBy(o => o.DistanceSquaredTo(this)))
        {
            var obscured = false;

            var relative = Vector2.Transform(obj.Position - Position, matrix);
            var rads = MathF.Atan2(relative.Y, relative.X);
            if (rads > fov || rads < -fov)
                obscured = true;
            else
            {
                var ray = Position.CastRay(obj.Position, false, true);
                foreach (var cell in ray)
                {
                    if (parentMap.WallLayer.GetTileValue((int)(cell.X / 16), (int)(cell.Y / 16)) > 0)
                    {
                        obscured = true;
                        break;
                    }
                }
            }
            if (!obscured)
                yield return obj;
        }
    }
}
