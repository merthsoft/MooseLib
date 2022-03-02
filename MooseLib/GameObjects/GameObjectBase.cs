using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.Interface;
using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.MooseEngine.GameObjects;

public abstract class GameObjectBase : IComparable<GameObjectBase>, IEquatable<GameObjectBase>
{
    public Guid Id { get; } = Guid.NewGuid();

    public GameObjectDef Def { get; set; }

    public int Layer { get; set; }

    public Vector2 Position { get; set; }
    public Vector2 WorldSize { get; set; }
    public float Rotation { get; set; }

    public virtual RectangleF WorldRectangle => new(Position.X, Position.Y, WorldSize.X, WorldSize.Y);

    public bool Remove { get; set; }

    public string State { get; set; } = "";

    public string? Direction { get; set; }

    public Action? StateCompleteAction { get; set; }

    public IMap? ParentMap { get; set; }

    public List<Tween> ActiveTweens { get; } = new();

    public GameObjectBase(GameObjectDef def, Vector2? position = null, int? layer = null, Vector2? size = null, string? direction = null)
    {
        Def = def;
        Layer = layer ?? Def.DefaultLayer;
        Position = position ?? Def.DefaultPosition;
        WorldSize = size ?? Def.DefaultSize;
        Direction = direction;
    }

    public abstract void Update(GameTime gameTime);

    public void ClearCompletedTweens()
        => ActiveTweens.RemoveAll(t => !t.IsAlive);

    public virtual void PostUpdate()
    {
        if (Remove)
            ClearTweens();
        else
            ClearCompletedTweens();
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void OnAdd() { }

    public virtual void OnRemove() { }

    public Tween TweenToPosition(Vector2 toValue,
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null)
        => ActiveTweens.AddItem(
            MooseGame.Instance.Tween(this, o => o.Position,
                toValue, duration, delay, onEnd, onBegin, repeatCount, repeatDelay, autoReverse, easingFunction
            ));

    public Tween TweenToSize(Vector2 toValue,
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null)
        => ActiveTweens.AddItem(
            MooseGame.Instance.Tween(this, o => o.WorldSize,
                toValue, duration, delay, onEnd, onBegin, repeatCount, repeatDelay, autoReverse, easingFunction
            ));

    public Tween TweenToRotation(float toValue,
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null)
        => ActiveTweens.AddItem(
            MooseGame.Instance.Tween(this, o => o.Rotation,
                toValue, duration, delay, onEnd, onBegin, repeatCount, repeatDelay, autoReverse, easingFunction
            ));

    public void ClearTweens(bool complete = false)
    {
        foreach (var tween in ActiveTweens.Where(t => t.IsAlive))
        {
            if (complete)
                tween.CancelAndComplete();
            else
                tween.Cancel();
        }

        ActiveTweens.Clear();
    }

    public virtual bool AtWorldPosition(Vector2 worldPosition)
        => WorldRectangle.Contains(worldPosition);

    public Vector2 GetCell(IMap? parentMap = null)
        => new Vector2(
            Position.X / (parentMap ?? ParentMap)!.TileWidth,
            Position.Y / (parentMap ?? ParentMap)!.TileHeight
        ).GetFloor();

    public bool InCell(int x, int y, IMap? parentMap = null)
        => Position.X / (parentMap ?? ParentMap)!.TileWidth == x
        && Position.Y / (parentMap ?? ParentMap)!.TileHeight == y;

    public bool InCell(int layer, int x, int y, IMap parentMap)
        => layer == Layer && InCell(x, y, parentMap);

    public int CompareTo(GameObjectBase? other)
        => Id.CompareTo(other?.Id);

    public bool Equals(GameObjectBase? other)
        => Id.Equals(other?.Id);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || (obj is GameObjectBase gameObject && gameObject.Id.Equals(Id));

    public override int GetHashCode()
        => Id.GetHashCode();

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
