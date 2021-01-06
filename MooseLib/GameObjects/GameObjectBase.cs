using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MooseLib.Defs;
using System;

namespace MooseLib.GameObjects
{
    public abstract class GameObjectBase : IComparable<GameObjectBase>
    {
        public MooseGame ParentGame { get; set; }
        public GameObjectDef Def { get; set; }

        public int Layer { get; set; }

        public Vector2 WorldPosition { get; set; }
        public Vector2 WorldSize { get; set; }
        public virtual RectangleF WorldRectangle => new(WorldPosition.X, WorldPosition.Y, WorldSize.X, WorldSize.Y);

        public bool RemoveFlag { get; set; }

        public string State { get; set; } = "";

        public Action? StateCompleteAction { get; set; }

        public GameObjectBase(MooseGame parentGame, GameObjectDef def, Vector2? position = null, int? layer = null, Vector2? size = null)
        {
            ParentGame = parentGame;
            Def = def;
            Layer = layer ?? Def.DefaultLayer;
            WorldPosition = position ?? Def.DefaultPosition;
            WorldSize = size ?? Def.DefaultSize;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual bool AtWorldPosition(Vector2 worldPosition)
            => WorldRectangle.Contains(worldPosition);

        public Vector2 GetCell()
            => new Vector2(WorldPosition.X / ParentGame.TileWidth, WorldPosition.Y / ParentGame.TileHeight).GetFloor();

        public bool InCell(int x, int y)
            => WorldPosition.X / ParentGame.TileWidth == x
            && WorldPosition.Y / ParentGame.TileHeight == y;

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