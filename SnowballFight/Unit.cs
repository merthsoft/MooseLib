using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MooseLib;
using MooseLib.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random.Distributions.Continuous;

namespace SnowballFight
{
    internal class Unit : AnimatedGameObject
    {
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
        private Unit? targettedUnit;

        public Unit(SnowballFightGame parentGame, UnitDef unitDef, int cellX, int cellY, string state) 
            : base(parentGame, unitDef, new(cellX * parentGame.TileWidth, cellY * parentGame.TileHeight), parentGame.UnitLayer, state: state)
        {
            UnitDef = unitDef;
            ParentGame = parentGame;
        }

        public override void Update(GameTime gameTime)
        {
            if (State == "walk")
                if (MoveDirection != Vector2.Zero)
                {
                    WorldPosition += MoveDirection;
                    if (WorldPosition == NextLocation)
                        MoveDirection = Vector2.Zero;
                }
                else if (MoveQueue.Count == 0)
                    State = "idle";
                else
                {
                    var nextCell = MoveQueue.Dequeue();
                    var cell = GetCell();
                    MoveDirection = new(nextCell.X - cell.X, nextCell.Y - cell.Y);
                    NextLocation = nextCell * ParentGame.TileSize;
                }

            base.Update(gameTime);
        }

        public void Attack(Unit targettedUnit)
        {
            this.targettedUnit = targettedUnit;
            State = "attack";
            StateCompleteAction = AttackComplete;
        }

        private void AttackComplete()
        {
            if (targettedUnit == null)
                return;

            var selectedUnitCell = GetCell();
            var targettedUnitCell = targettedUnit.GetCell();

            var startWorldPosition = WorldPosition + ParentGame.HalfTileSize;
            var wiggle = AimDistribution.NextDouble();
            var endWorldPosition = (targettedUnit.WorldPosition + ParentGame.HalfTileSize).RotateAround(startWorldPosition, (float)wiggle);
            var flightPath = ParentGame
                                .FindWorldRay(startWorldPosition, endWorldPosition.GetFloor())
                                .SkipWhile(inSelectedUnitCell)
                                .TakeWhile(notBlocked)
                                .Select(pos => pos.worldPosition);
            var snowBall = new Snowball(ParentGame, ParentGame.SnowballDef, startWorldPosition, flightPath);
            ParentGame.ObjectsToAdd.Enqueue(snowBall);
            State = "idle";
            StateCompleteAction = null;
            targettedUnit = null;

            bool inSelectedUnitCell((Vector2 worldPosition, IList<int> blockedVector) pos) => (pos.worldPosition / ParentGame.TileSize).GetFloor() == selectedUnitCell;

            bool notBlocked((Vector2, IList<int> blockedVector) pos) => pos.blockedVector.Skip(2).All(b => b == 0);
        }
    }
}
