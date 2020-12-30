using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MooseLib.GameObjects
{
    public abstract class GameObjectBase
    {
        public MooseGame ParentGame { get; set; }

        public int Layer { get; set; }

        public Vector2 WorldPosition { get; set; }

        public bool RemoveFlag { get; set; }

        public string State { get; set; } = "";

        public Action? StateCompleteAction { get; set; }

        public GameObjectBase(MooseGame parentGame, int layer = 0, Vector2? position = null)
            => (ParentGame, Layer, WorldPosition)
             = (parentGame, layer, position ?? Vector2.Zero);

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);

        public bool AtWorldLocation(Vector2 worldLocation)
            => worldLocation.X >= WorldPosition.X && worldLocation.X < WorldPosition.X + 16
            && worldLocation.Y >= WorldPosition.Y && worldLocation.Y < WorldPosition.Y + 16;

        public int CompareTo(AnimatedGameObject? other)
            => other != null 
                ? Layer == other.Layer 
                    ? WorldPosition.Y == other.WorldPosition.Y
                        ? WorldPosition.X == other.WorldPosition.X
                            ? GetHashCode().CompareTo(other.GetHashCode()) 
                            : WorldPosition.X.CompareTo(other.WorldPosition.X)
                        : WorldPosition.Y.CompareTo(other.WorldPosition.Y)
                    : Layer.CompareTo(other.Layer) 
                : 1;


        public Vector2 GetCell()
            => new Vector2(WorldPosition.X / ParentGame.TileWidth, WorldPosition.Y / ParentGame.TileHeight).GetFloor();

        public bool InCell(int x, int y)
            => WorldPosition / new Vector2(ParentGame.TileWidth, ParentGame.TileHeight) == new Vector2(x, y);
    }
}