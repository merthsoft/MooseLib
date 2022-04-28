namespace Merthsoft.Moose.Rays.Actors;
public record ActorDef(string DefName, string DefaultState = ActorStates.ChaseState)
    : RayGameObjectDef(DefName, 0, ObjectRenderMode.Directional)
{
    public Dictionary<string, List<ActorFrame>> States = new();
    public int Health;
    public Texture2D Texture = null!;
    public int FrameCount;
    public int SpriteSize = 16;

    public override void LoadContent(MooseContentManager contentManager)
    {
        if (ObjectRenderMode is not ObjectRenderMode.Directional)
            return;

        Texture = contentManager.LoadImage($"Actors\\{DefName}");
        FrameCount = Texture.Width / SpriteSize;
    }
}

public class Actor : RayGameObject
{
    public ActorDef ActorDef;
    public int Health = 1;
    public int FrameIndex = 0;
    public double FrameTimer = 0;
    public bool Aware = false;

    public bool Shootable => ActorFrame?.Shootable ?? false;
    public override bool Blocking => ActorFrame?.Blocking ?? false;
    
    public List<ActorFrame> CurrentState => ActorDef.States[State];
    public ActorFrame ActorFrame => CurrentState[FrameIndex];

    private string state = "";
    public new string State {
        get => state; 
        set {
            if (State != value)
                FrameIndex = 0;
            base.State = value;
            state = value;
        }
    }

    public Actor(ActorDef def, int x, int y) : base(def, x, y)
    {
        ActorDef = def;
        State = def.DefaultState;
        Health = def.Health;
    }

    public void Initialize(string? state = null)
    {
        State = state ?? State;
        FrameIndex = 0;
        ObjectRenderMode = ActorFrame?.RenderMode ?? ActorDef.ObjectRenderMode;
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

            if (ActorFrame.NextState == null)
            {
                FrameIndex++;
                if (FrameIndex >= CurrentState.Count)
                    FrameIndex = 0;

                ActorFrame.StartAction?.Invoke(this);
                ObjectRenderMode = ActorFrame?.RenderMode ?? ActorDef.ObjectRenderMode;
            } else
            {
                State = ActorFrame.NextState;
                FrameIndex = 0;
            }
            FrameTimer = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (!Aware)
            Health = 0;
        
        State = "Hit";
        Aware = true;        
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

    public virtual void Interact() { }

    public override void PreUpdate(MooseGame game, GameTime gameTime)
    {
        if (State != PreviousState)
        {
            ActorFrame?.EndAction?.Invoke(this);
            FrameIndex = 0;
            ActorFrame?.StartAction?.Invoke(this);
            ObjectRenderMode = ActorFrame?.RenderMode ?? ActorDef.ObjectRenderMode;
            FrameTimer = 0;
        }
        base.PreUpdate(game, gameTime);
    }

    public static void RemoveActor(Actor a)
        => a.Remove = true;
}
