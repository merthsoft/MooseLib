using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.Interface;
using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.MooseEngine.GameObjects;

public abstract class GameObjectBase : ITweenOwner, IComparable<GameObjectBase>, IEquatable<GameObjectBase>
{
    public Guid Id { get; } = Guid.NewGuid();

    public GameObjectDef Def { get; set; }

    public string Layer { get; set; }

    int tileWidth = 1;
    int tileHeight = 1;

    private Vector2 position;
    public Vector2 Position {
        get => position;
        set {
            position = value;

            CellCache.X = (int)Position.X / tileWidth;
            CellCache.Y = (int)Position.Y / tileHeight;
        }
    }

    private Point CellCache;

    public Point Cell => CellCache;

    public Vector2 WorldSize { get; set; }
    public float Rotation { get; set; }
    public SpriteEffects Effects { get; set; }
    public Vector2 DrawOffset { get; set; }
    public Vector2 Scale { get; set; }
    public Vector2 Origin { get; set; }
    public float LayerDepth { get; set; } = .5f;

    private Color color = Color.White;
    public Color Color
    {
        get => color;
        set
        {
            color = value;
            colorHsl = color.ToHsl();
        }
    }

    private HslColor colorHsl;

    public float ColorA
    {
        get => color.A;
        set => color.A = (byte)(value * 255);
    }

    public float ColorH
    {
        get => colorHsl.H;
        set
        {
            var a = Color.A;
            colorHsl = new HslColor(value, colorHsl.S, colorHsl.L);
            color = colorHsl.ToRgb();
            color.A = a;
        }
    }

    public float ColorS
    {
        get => colorHsl.S;
        set
        {
            var a = Color.A;
            colorHsl = new HslColor(colorHsl.H, value, colorHsl.L);
            color = colorHsl.ToRgb();
            color.A = a;
        }
    }

    public float ColorL
    {
        get => colorHsl.L;
        set
        {
            var a = Color.A;
            colorHsl = new HslColor(colorHsl.H, colorHsl.S, value);
            color = colorHsl.ToRgb();
            color.A = a;
        }
    }

    public virtual RectangleF WorldRectangle => new(Position.X, Position.Y, 
        WorldSize.X * Scale.X, WorldSize.Y * Scale.Y);

    public bool Remove { get; set; }

    public string State { get; set; } = "";
    public string PreviousState { get; set; } = "";

    public string Direction { get; set; } = "";

    public IMap ParentMap { get; private set; } = null!;

    public List<Tween> ActiveTweens { get; } = new();

    public Dictionary<string, Func<MooseGame, GameTime, string>> StateMap { get; } = new();

    public GameObjectBase(GameObjectDef def, Vector2? position = null, string direction = "", float? rotation = null, Vector2? size = null, string? layer = null)
    {
        Def = def;
        Layer = layer ?? Def.DefaultLayer;
        Position = position ?? Def.DefaultPosition;
        WorldSize = size ?? Def.DefaultSize;
        Rotation = rotation ?? Def.DefaultRotation;
        
        Scale = Def.DefaultScale;
        Origin = Def.DefaultOrigin;

        Direction = direction;

        StateMap[State] = EmptyState;
    }

    public virtual string EmptyState(MooseGame mooseGame, GameTime gameTime)
        => "";

    public virtual void SetMap(IMap map)
        => ParentMap = map;

    public virtual bool PreUpdate(MooseGame mooseGame, GameTime gameTime)
        => true;

    public virtual void Update(MooseGame game, GameTime gameTime)
    {
        var state = State;
        State = StateMap[State](game, gameTime);
        PreviousState = state;
    }

    public virtual void PostUpdate(MooseGame game, GameTime gameTime)
    {
        if (Remove)
            this.ClearTweens();
        else
            this.ClearCompletedTweens();
    }

    public abstract void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch);

    public virtual void OnAdd() {
        tileWidth = ParentMap.TileWidth;
        tileHeight = ParentMap.TileHeight;

        CellCache.X = (int)Position.X / tileWidth;
        CellCache.Y = (int)Position.Y / tileHeight;
    }

    public virtual void OnRemove() { }

    public Tween TweenToPosition(Vector2 toValue,
        float duration, float delay = 0f,
        Action<Tween>? onEnd = null, Action<Tween>? onBegin = null,
        int repeatCount = 0, float repeatDelay = 0f,
        bool autoReverse = false, Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.Position, toValue, duration, delay, onEnd, onBegin, 
                repeatCount, repeatDelay, autoReverse, easingFunction);

    public Tween TweenToScale(Vector2 toValue,
        float duration, float delay = 0f,
        Action<Tween>? onEnd = null, Action<Tween>? onBegin = null,
        int repeatCount = 0, float repeatDelay = 0f,
        bool autoReverse = false, Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.Scale, toValue, duration, delay, onEnd, onBegin, 
            repeatCount, repeatDelay, autoReverse, easingFunction);

    public Tween TweenToRotation(float toValue,
        float duration, float delay = 0f,
        Action<Tween>? onEnd = null, Action<Tween>? onBegin = null,
        int repeatCount = 0, float repeatDelay = 0f,
        bool autoReverse = false, Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.Rotation, toValue, duration, delay, onEnd, onBegin, 
            repeatCount, repeatDelay, autoReverse, easingFunction);

    public Tween TweenToAlpha(float toValue,
        float duration, float delay = 0f,
        Action<Tween>? onEnd = null, Action<Tween>? onBegin = null,
        int repeatCount = 0, float repeatDelay = 0f,
        bool autoReverse = false, Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.ColorA, toValue, duration, delay, onEnd, onBegin,
            repeatCount, repeatDelay, autoReverse, easingFunction);

    public virtual bool AtWorldPosition(Vector2 worldPosition)
        => WorldRectangle.Contains(worldPosition);

    public bool InCell(int x, int y)
        => Position.X / tileWidth == x
        && Position.Y / tileHeight == y;

    public bool InCell(string layer, int x, int y)
        => layer == Layer && InCell(x, y);

    public int CompareTo(GameObjectBase? other)
        => Id.CompareTo(other?.Id);

    public bool Equals(GameObjectBase? other)
        => Id.Equals(other?.Id);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || (obj is GameObjectBase gameObject && gameObject.Id.Equals(Id));

    public override int GetHashCode()
        => Id.GetHashCode();

    public float DistanceTo(GameObjectBase obj)
    {
        var (x1, y1) = obj.Position;
        var (x2, y2) = Position;
        var xDiff = x2 - x1;
        var yDiff = y2 - y1;

        return MathF.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    public float DistanceSquaredTo(GameObjectBase obj)
    {
        var (x1, y1) = obj.Position;
        var (x2, y2) = Position;
        var xDiff = x2 - x1;
        var yDiff = y2 - y1;

        return xDiff * xDiff + yDiff * yDiff;
    }

    public float DistanceSquaredTo(Vector2 position)
    {
        var (x1, y1) = position;
        var (x2, y2) = Position;
        var xDiff = x2 - x1;
        var yDiff = y2 - y1;

        return xDiff * xDiff + yDiff * yDiff;
    }

    public static bool operator ==(GameObjectBase? left, GameObjectBase? right)
        => left is null ? right is null : left.Equals(right);

    public static bool operator !=(GameObjectBase? left, GameObjectBase? right)
        => !(left == right);

    public static bool operator <(GameObjectBase? left, GameObjectBase? right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(GameObjectBase? left, GameObjectBase? right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(GameObjectBase? left, GameObjectBase? right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(GameObjectBase? left, GameObjectBase? right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
}
