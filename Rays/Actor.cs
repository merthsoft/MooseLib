namespace Merthsoft.Moose.Rays;
public record ActorDef(string DefName, int DefaultTextureIndex, string DefaultState = ActorStates.StandState, int RenderBottom = 0, int RenderTop = 16)
    : RayGameObjectDef(DefName, DefaultTextureIndex, ObjectRenderMode.Directional, RenderBottom, RenderTop)
{
    public Dictionary<string, ActorFrame> States = new Dictionary<string, ActorFrame>();
    public int Health;
}

public class Actor : RayGameObject
{
    public ActorDef ActorDef;
    public int Health = 1;
    public ActorFrame? ActorFrame;
    public double FrameTimer = 0;
    public bool RestartState = false;

    public bool Shootable => ActorFrame?.Shootable ?? false;

    public Actor(ActorDef def, int x, int y) : base(def, x, y)
    {
        ActorDef = def;
        State = def.DefaultState;
        Health = def.Health;
    }

    public void Initialize(string? state = null)
    {
        State = state ?? State;
        ActorFrame = ActorDef.States[State];
        ObjectRenderMode = ActorFrame?.RenderMode ?? ActorDef.RenderMode;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        FrameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        TextureIndexOffset = ActorFrame?.FrameOffset ?? 0;

        if (ActorFrame == null)
            return;

        if (FrameTimer >= ActorFrame.Length)
        {
            ActorFrame.EndAction?.Invoke(this);
            var next = ActorFrame.Next;
            if (next == null && ActorFrame.NextState != null)
            {
                State = ActorFrame.NextState;
                RestartState = true;
            }
            else if (next != null)
            {
                ActorFrame = next;
                ActorFrame.StartAction?.Invoke(this);
                ObjectRenderMode = ActorFrame?.RenderMode ?? ActorDef.RenderMode;
            }
            FrameTimer = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        State = "Hit";
    }

    public static void PostHit(Actor actor)
    {
        if (actor.Health <= 0)
            actor.State = ActorStates.DyingState;
        else
            actor.State = ActorStates.ChaseState;
    }

    public static void Rotate(Actor actor)
    {
        var rot = Matrix.CreateRotationZ(MathF.PI / 4);
        actor.FacingDirection = Vector3.Transform(actor.FacingDirection, rot);
    }

    public override void PreUpdate(MooseGame game, GameTime gameTime)
    {
        if (State != PreviousState || RestartState)
        {
            RestartState = false;
            ActorFrame?.EndAction?.Invoke(this);
            ActorFrame = ActorDef.States[State];
            ActorFrame?.StartAction?.Invoke(this);
            ObjectRenderMode = ActorFrame?.RenderMode ?? ActorDef.RenderMode;
            FrameTimer = 0;
        }
        base.PreUpdate(game, gameTime);
    }
}

public record ActorFrame(int FrameOffset = 0, float Length = 0, 
    bool Shootable = false, string? NextState = null, ObjectRenderMode? RenderMode = null,
    Action<Actor>? StartAction = null, Action<Actor>? EndAction = null)
{
    public ActorFrame? Next { get; set; }

    public ActorFrame AddNext(Func<ActorFrame> add)
    {
        Next = add();
        return this;
    }

    public ActorFrame ChainNext(ActorFrame next)
    {
        Next = next;
        return next;
    }
}
