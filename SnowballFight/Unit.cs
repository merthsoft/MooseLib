using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MooseLib;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random.Distributions.Continuous;

namespace SnowballFight
{
    class Unit : GameObject
    {

        private readonly NormalDistribution AimDistribution = new NormalDistribution(0, 2);

        private class States
        {
            public const string Idle = "idle";
            public const string Walk = "walk";
        }

        public int Speed { get; set; }

        public Queue<Vector2> MoveQueue { get; } = new Queue<Vector2>();
        private Vector2 MoveDirection = Vector2.Zero;
        private Vector2 NextLocation = Vector2.Zero;

        public Unit(MooseGame parentGame, SpriteSheet spriteSheet, int cellX, int cellY, string direction, string state) 
            : base(parentGame, spriteSheet, new(cellX * 16, cellY * 16), new(8, 8), direction, state, SnowballFightGame.UnitLayer) { }

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

        public void Attack(Unit TargettedUnit)
        {
            State = "attack";
            StateCompleteAction = () =>
            {
                var selectedUnitCell = GetCell();
                var targettedUnitCell = TargettedUnit.GetCell();

                var startWorldPosition = Position + ParentGame.HalfTileSize;
                var wiggle = AimDistribution.NextDouble();
                var endWorldPosition = (TargettedUnit.Position + ParentGame.HalfTileSize).RotateAround(startWorldPosition, (float)wiggle);
                var flightPath = ParentGame.FindWorldRay(startWorldPosition, endWorldPosition.GetFloor());
                var snowBall = new Snowball(ParentGame, startWorldPosition,
                    flightPath
                        .SkipWhile(pos => (pos.worldPosition / ParentGame.TileSize).GetFloor() == selectedUnitCell)
                        .TakeWhile(pos => pos.blockedVector.Skip(2).Sum() == 0 || (pos.worldPosition / ParentGame.TileSize).GetFloor() == targettedUnitCell)
                        .Select(pos => pos.worldPosition)
                );
                ParentGame.UpdateObjects.Enqueue(snowBall);
                State = "idle";
                StateCompleteAction = null;
            };

        }
    }
}
