using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MooseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random.Distributions.Continuous;

namespace SnowballFight
{
    class Unit : GameObject
    {

        private static readonly Vector2 spriteOffset = new(8, 8);
        
        public class States
        {
            public const string Idle = "idle";
            public const string Walk = "walk";
        }

        public new SnowballFightGame ParentGame { get; private set; }

        public UnitDef UnitDef { get; }
        public Texture2D Portrait => UnitDef.Portrait;
        public int DisplaySpeed => UnitDef.Speed;
        public string DisplayName => UnitDef.DisplayName;
        public int DisplayHealth => UnitDef.MaxHealth;
        public int DisplayMaxHealth => UnitDef.MaxHealth;
        public float DisplayAccuracy => MathF.Round(10 - 5 * UnitDef.AccuracySigma, 1);

        public Queue<Vector2> MoveQueue { get; } = new Queue<Vector2>();
        private Vector2 MoveDirection = Vector2.Zero;
        private Vector2 NextLocation = Vector2.Zero;
        
        private readonly NormalDistribution AimDistribution = new(0, 2);

        public Unit(SnowballFightGame parentGame, UnitDef unitDef, int cellX, int cellY, string state) 
            : base(parentGame, unitDef.AnimationKey, new(cellX * 16, cellY * 16), spriteOffset, state, parentGame.UnitLayer) 
        {
            UnitDef = unitDef;
            ParentGame = parentGame;
        }

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
            StateCompleteAction = () => AttackComplete(TargettedUnit);

        }

        private void AttackComplete(Unit TargettedUnit)
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
        }
    }
}
