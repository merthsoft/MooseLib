using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MooseLib;
using System.Collections.Generic;

namespace SnowballFight
{
    class Unit : GameObject
    {
        public int Speed { get; set; }

        public Queue<Vector2> MoveQueue { get; } = new Queue<Vector2>();
        private Vector2 MoveDirection = Vector2.Zero;
        private Vector2 NextLocation = Vector2.Zero;

        public Unit(MooseGame parentGame, SpriteSheet spriteSheet, int cellX, int cellY, string direction, string state, int layer) 
            : base(parentGame, spriteSheet, cellX, cellY, direction, state, layer) { }

        public override void Update(GameTime gameTime)
        {

            if (State == "walk")
            {
                if (MoveDirection != Vector2.Zero)
                {
                    Position += MoveDirection;
                    if (Position == NextLocation)
                        MoveDirection = Vector2.Zero;
                }
                else if (MoveQueue.Count == 0)
                {
                    State = "idle";
                }
                else
                {
                    var nextCell = MoveQueue.Dequeue();
                    var cell = GetCell();
                    MoveDirection = new(nextCell.X - cell.X, nextCell.Y - cell.Y);
                    NextLocation = nextCell * ParentGame.TileSize;
                }
            }

            base.Update(gameTime);
        }
    }
}
