﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MooseLib.Defs;
using MooseLib.Interface;
using System;

namespace MooseLib.GameObjects
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

        public Vector2 GetCell(IMap parentMap)
            => new Vector2(WorldPosition.X / parentMap.TileWidth, WorldPosition.Y / parentMap.TileHeight).GetFloor();

        public bool InCell(int x, int y, IMap parentMap)
            => WorldPosition.X / parentMap.TileWidth == x
            && WorldPosition.Y / parentMap.TileHeight == y;

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