using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MooseLib
{
    public class Unit
    {
        public MooseGame ParentGame { get; set; }

        public Vector2 Location { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public Direction Direction { get; set; } = Direction.Down;

        public State State { get; set; } = State.Idle;

        public int Speed { get; set; }

        public SpriteEffects SpriteEffects { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;

        public Queue<Vector2> MoveQueue { get; } = new Queue<Vector2>();
        private Vector2 MoveDirection = Vector2.Zero;
        private Vector2 NextLocation = Vector2.Zero;

        private string PlayKey
            => Direction == Direction.None
                ? State.ToString().ToLower()
                : $"{State.ToString().ToLower()}_{Direction}";
        
        private string PreviousPlayKey = "";

        private readonly Vector2 SpriteOffset;

        public Unit(MooseGame parentGame, SpriteSheet spriteSheet, int cellX, int cellY, Direction direction = Direction.Down, State state = State.Idle)
        {
            Sprite = new AnimatedSprite(spriteSheet);
            Location = new(cellX * 16, cellY * 16);
            SpriteOffset = new Vector2(8, 8);
            Direction = direction;
            State = state;
            ParentGame = parentGame;
        }

        public void Draw(SpriteBatch spriteBatch)
            => Sprite.Draw(spriteBatch, Location + SpriteOffset, Rotation, Scale, SpriteEffects);

        public void Update(GameTime gameTime)
        {
            if (State == State.Walk)
            {
                if (MoveDirection != Vector2.Zero)
                {
                    Location += MoveDirection;
                    if (Location == NextLocation)
                        MoveDirection = Vector2.Zero;
                }
                else if (MoveQueue.Count == 0)
                {
                    State = State.Idle;
                }
                else
                {
                    var nextCell = MoveQueue.Dequeue();
                    var cell = GetCell();
                    MoveDirection = new(nextCell.X - cell.X, nextCell.Y - cell.Y);
                    NextLocation = nextCell * ParentGame.TileSize;
                }
            }

            if (PlayKey != PreviousPlayKey)
            {
                Sprite.Play(PlayKey);
                PreviousPlayKey = PlayKey;
            }

            Sprite.Update(gameTime);
        }

        public bool Clicked(Vector2 worldLocation)
            => worldLocation.X >= Location.X && worldLocation.X < (Location.X + 16)
            && worldLocation.Y >= Location.Y && worldLocation.Y < (Location.Y + 16);

        public Vector2 GetCell()
            => new((int)(Location.X / ParentGame.TileWidth), (int)(Location.Y / ParentGame.TileHeight));

        public bool InCell(int x, int y)
            => (Location / new Vector2(ParentGame.TileWidth, ParentGame.TileHeight)) == new Vector2(x, y);
    }
}
