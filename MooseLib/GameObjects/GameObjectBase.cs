using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.GameObjects
{
    public abstract class GameObjectBase : IComparable<GameObjectBase>, IEquatable<GameObjectBase>
    {
        public Guid Id { get; } = Guid.NewGuid();

        public GameObjectDef Def { get; set; }

        public int Layer { get; set; }

        public Vector2 WorldPosition { get; set; }
        public Vector2 WorldSize { get; set; }
        public virtual RectangleF WorldRectangle => new(WorldPosition.X, WorldPosition.Y, WorldSize.X, WorldSize.Y);

        public bool RemoveFlag { get; set; }

        public string State { get; set; } = "";

        public string? Direction { get;set; }

        public Action? StateCompleteAction { get; set; }

        public IMap? ParentMap { get; set; }

        public GameObjectBase(GameObjectDef def, Vector2? position = null, int? layer = null, Vector2? size = null, string? direction = null)
        {
            Def = def;
            Layer = layer ?? Def.DefaultLayer;
            WorldPosition = position ?? Def.DefaultPosition;
            WorldSize = size ?? Def.DefaultSize;
            Direction = direction;
        }

        public virtual void Update(GameTime gameTime)
        {
            WorldPosition = WorldPosition.Round(Def.WorldSizeRound);
        }

        public abstract DrawParameters GetDrawParameters();

        public virtual void OnAdd() { }

        public virtual void OnRemove() { }

        public virtual bool AtWorldPosition(Vector2 worldPosition)
            => WorldRectangle.Contains(worldPosition);

        public Vector2 GetCell(IMap? parentMap = null)
            => new Vector2(
                WorldPosition.X / (parentMap ?? ParentMap)!.TileWidth, 
                WorldPosition.Y / (parentMap ?? ParentMap)!.TileHeight
            ).GetFloor();

        public bool InCell(int x, int y, IMap? parentMap = null)
            => WorldPosition.X / (parentMap ?? ParentMap)!.TileWidth == x
            && WorldPosition.Y / (parentMap ?? ParentMap)!.TileHeight == y;

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

        public static bool operator ==(GameObjectBase left, GameObjectBase right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(GameObjectBase left, GameObjectBase right) 
            => !(left == right);

        public static bool operator <(GameObjectBase left, GameObjectBase right) 
            => left is null ? right is not null : left.CompareTo(right) < 0;

        public static bool operator <=(GameObjectBase left, GameObjectBase right) 
            => left is null || left.CompareTo(right) <= 0;

        public static bool operator >(GameObjectBase left, GameObjectBase right) 
            => left is not null && left.CompareTo(right) > 0;

        public static bool operator >=(GameObjectBase left, GameObjectBase right) 
            => left is null ? right is null : left.CompareTo(right) >= 0;
    }
}