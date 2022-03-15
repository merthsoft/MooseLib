using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.Interface;
using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.MooseEngine.GameObjects;

public abstract class GameObjectBase : ITweenOwner, IComparable<GameObjectBase>, IEquatable<GameObjectBase>
{
    public Guid Id { get; } = Guid.NewGuid();

    public GameObjectDef Def { get; set; }

    public string Layer { get; set; }

    int tileWidth;
    int tileHeight;

    public Vector2 Position { get; set; }
    public Vector2 WorldSize { get; set; }
    public float Rotation { get; set; }
    public SpriteEffects Effects { get; set; }
    public Vector2 DrawOffset { get; set; }
    public Vector2 Scale { get; set; }
    public Vector2 Origin { get; set; }

    public virtual RectangleF WorldRectangle => new(Position.X, Position.Y, 
        WorldSize.X * Scale.X, WorldSize.Y * Scale.Y);

    public bool Remove { get; set; }

    public string State { get; set; } = "";

    public string? Direction { get; set; }

    public Action? StateCompleteAction { get; set; }

    public IMap ParentMap { get; set; } = null!;

    public List<Tween> ActiveTweens { get; } = new();

    public GameObjectBase(GameObjectDef def, Vector2? position = null, string? direction = null, float? rotation = null, Vector2? size = null, string? layer = null)
    {
        Def = def;
        Layer = layer ?? Def.DefaultLayer;
        Position = position ?? Def.DefaultPosition;
        WorldSize = size ?? Def.DefaultSize;
        Rotation = rotation ?? Def.DefaultRotation;
        
        Scale = Def.DefaultScale;
        Origin = Def.DefaultOrigin;
        
        Direction = direction;
    }

    public abstract void Update(MooseGame game, GameTime gameTime);

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
    
    public virtual bool AtWorldPosition(Vector2 worldPosition)
        => WorldRectangle.Contains(worldPosition);

    public Point GetCell()
        => new(
            (int)Position.X / tileWidth,
            (int)Position.Y / tileHeight);

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
