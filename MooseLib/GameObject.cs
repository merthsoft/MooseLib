﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;

namespace MooseLib
{
    public class GameObject : IComparable<GameObject>
    {
        public MooseGame ParentGame { get; set; }

        public Vector2 Position { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public string Direction { get; set; }

        public string State { get; set; }

        public SpriteEffects SpriteEffects { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public int Layer { get; set; }

        private string PlayKey
            => Direction == MooseLib.Direction.None
                ? State.ToLower()
                : $"{State.ToLower()}_{Direction.ToLower()}";
        
        private string PreviousPlayKey = "";

        private readonly Vector2 SpriteOffset;
        
        public GameObject(MooseGame parentGame, SpriteSheet spriteSheet, Vector2 position, Vector2 spriteOffset, string direction = MooseLib.Direction.None, string state = "idle", int layer = 0)
        {
            Sprite = new AnimatedSprite(spriteSheet);
            Position = position;
            SpriteOffset = spriteOffset;
            Direction = direction;
            State = state;
            ParentGame = parentGame;
            Layer = layer;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
            => Sprite.Draw(spriteBatch, Position + SpriteOffset, Rotation, Scale, SpriteEffects);

        public virtual void Update(GameTime gameTime)
        {
            if (PlayKey != PreviousPlayKey)
            {
                Sprite.Play(PlayKey);
                PreviousPlayKey = PlayKey;
            }

            Sprite.Update(gameTime);
        }

        public bool AtWorldLocation(Vector2 worldLocation)
            => worldLocation.X >= Position.X && worldLocation.X < (Position.X + 16)
            && worldLocation.Y >= Position.Y && worldLocation.Y < (Position.Y + 16);

        public Vector2 GetCell()
            => new Vector2(Position.X / ParentGame.TileWidth, Position.Y / ParentGame.TileHeight).GetFloor();

        public bool InCell(int x, int y)
            => (Position / new Vector2(ParentGame.TileWidth, ParentGame.TileHeight)) == new Vector2(x, y);

        public int CompareTo(GameObject? other)
            => other == null ? 1 : Layer == other.Layer ? GetHashCode().CompareTo(other.GetHashCode()) : Layer.CompareTo(other.Layer);

    }
}
