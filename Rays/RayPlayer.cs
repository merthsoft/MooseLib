using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.Rays.Intersections;

namespace Merthsoft.Moose.Rays;
public class RayPlayer : GameObjectBase
{
    public static Vector2 StandardPlaneEffect(Vector2 pv)
            => new(-pv.Y, pv.X);

    public Vector2 Location { get; set; } = Vector2.Zero;

    private Vector2 direction = Vector2.Zero;
    public Vector2 DirectionVector
    {
        get => direction;
        set
        {
            direction = value;
            PlaneCache = null;
        }
    }

    private float fieldOfView = 0.66f;
    public float FieldOfView
    {
        get => fieldOfView;
        set
        {
            fieldOfView = value;
            PlaneCache = null;
        }
    }

    private Func<Vector2, Vector2> planeEffect = StandardPlaneEffect;
    public Func<Vector2, Vector2> PlaneEffect
    {
        get => planeEffect;
        set
        {
            planeEffect = value;
            PlaneCache = null;
        }
    }

    public Vector2? PlaneCache;

    public Vector2 Plane => PlaneCache ??= PlaneEffect(DirectionVector) * FieldOfView;

    public static List<Intersection> Intersections { get; } = new List<Intersection>();

    public RayPlayer(GameObjectDef def, Vector2 position, Vector2 directionVector) : base(def, position, layer:"objects")
    {
        DirectionVector = directionVector;
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch) { }
    public override void Update(MooseGame game, GameTime gameTime)
    {
        KeyboardUpdate();
        RayUpdate();
    }

    private void KeyboardUpdate()
    {
    }

    private void RayUpdate()
    {
        Intersections.Clear();
        var rayMap = (RayMap)ParentMap;
        var viewWidth = RayGame.Instance.ViewWidth;
        var fov = MathF.PI * FieldOfView;
        for (var x = 0f; x < viewWidth; x++)
        {
            var angle = fov * x / viewWidth;
            foreach (var pos in Position.CastRay(angle, false, true))
            {
                var cell = pos / 32;
                var wallNumber = rayMap.WallLayer.GetTileValue((int)cell.X, (int)cell.Y);
                if (wallNumber > -1)
                {
                    Intersections.Add(new(pos, wallNumber, false));
                    break;
                }
            }
        }
    }
}
