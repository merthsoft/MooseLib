using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Merthsoft.MooseEngine.Defs;
using Merthsoft.MooseEngine.Interface;
using System;

namespace Merthsoft.MooseEngine.GameObjects
{
    public abstract class GameObjectBase : IComparable<GameObjectBase>
    {
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

        public abstract void Draw(SpriteBatch spriteBatch);

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
            => (IsNull: other == null, Layer: Layer == other?.Layer, Y: WorldPosition.Y == other?.WorldPosition.Y, X: WorldPosition.X == other?.WorldPosition.X) switch
            {
                { IsNull: true } => 1,
                { Layer: false } => Layer.CompareTo(other!.Layer),
                { Y: false } => WorldPosition.Y.CompareTo(other!.WorldPosition.Y),
                { X: false } => WorldPosition.X.CompareTo(other!.WorldPosition.X),
                _ => GetHashCode().CompareTo(other!.GetHashCode()),
            };
    }
}