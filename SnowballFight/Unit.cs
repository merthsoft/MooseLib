using Merthsoft.MooseEngine;
using Merthsoft.MooseEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Troschuetz.Random.Distributions.Continuous;

namespace Merthsoft.SnowballFight
{
    internal class Unit : AnimatedGameObject
    {
        public class States
        {
            public const string Idle = "idle";
            public const string Walk = "walk";
            public const string Attack = "attack";
        }

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
        private bool stepFlag;

        public Unit(UnitDef unitDef, int worldX, int worldY, string state) 
            : base(unitDef, new(worldX, worldY), SnowballFightGame.UnitLayer, state: state)
        {
            UnitDef = unitDef;
        }

        public override void Update(GameTime gameTime)
        {
            if (State == States.Walk)
                if (MoveDirection != Vector2.Zero)
                {
                    takeStep();
                    if (stepFlag) 
                        takeStep();
                    stepFlag = !stepFlag;
                }
                else if (MoveQueue.Count == 0)
                    State = States.Idle;
                else
                {
                    var nextCell = MoveQueue.Dequeue();
                    var cell = GetCell();
                    MoveDirection = new(nextCell.X - cell.X, nextCell.Y - cell.Y);
                    NextLocation = nextCell * ParentMap!.TileSize;
                }

            base.Update(gameTime);

            void takeStep()
            {
                WorldPosition += MoveDirection;
                if (WorldPosition.GetFloor() == NextLocation.GetFloor())
                    MoveDirection = Vector2.Zero;
            }
        }

        public void Attack(Unit targettedUnit)
        {
            this.targettedUnit = targettedUnit;
            State = States.Attack;
            StateCompleteAction = AttackComplete;
        }

        private void AttackComplete()
        {
            if (targettedUnit != null)
            {
                var startWorldPosition = WorldPosition + ParentMap!.HalfTileSize;
                var wiggle = AimDistribution.NextDouble();
                var endWorldPosition = (targettedUnit.WorldPosition + ParentMap!.HalfTileSize).RotateAround(startWorldPosition, (float)wiggle);
                var flightPath = ParentMap!.FindWorldRay(startWorldPosition, endWorldPosition);
                SnowballFightGame.SpawnSnowball(flightPath.Select(p => p.WorldPosition));
            }

            State = States.Idle;
            StateCompleteAction = null;
            targettedUnit = null;
        }
    }
}
