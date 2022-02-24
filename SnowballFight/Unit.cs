using Merthsoft.Moose.MooseEngine.GameObjects;
using Troschuetz.Random.Distributions.Continuous;

namespace Merthsoft.Moose.SnowballFight;

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

    public int DisplaySpeed => UnitDef.MovementSpeed;

    public string DisplayName => UnitDef.DisplayName;

    public int DisplayHealth => UnitDef.MaxHealth;

    public int DisplayMaxHealth => UnitDef.MaxHealth;

    public float DisplayAccuracy => MathF.Round(10 - 5 * UnitDef.AccuracySigma, 1);

    public Queue<Vector2> MoveQueue { get; } = new Queue<Vector2>();
    private Vector2 MoveDirection = Vector2.Zero;
    private Vector2 NextLocation = Vector2.Zero;

    private readonly NormalDistribution AimDistribution;
    private Unit? targettedUnit;
    private bool stepFlag;

    public Unit(UnitDef unitDef, int worldX, int worldY, string state)
        : base(unitDef, new(worldX, worldY), SnowballFightGame.UnitLayer, state: state)
    {
        UnitDef = unitDef;
        AimDistribution = new NormalDistribution(0, unitDef.AccuracySigma);
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
            Position += MoveDirection;
            if (Position.GetFloor() == NextLocation.GetFloor())
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
            var startWorldPosition = Position + ParentMap!.HalfTileSize;
            var wiggle = AimDistribution.NextDouble();
            var endWorldPosition = (targettedUnit.Position + ParentMap!.HalfTileSize).RotateAround(startWorldPosition, (float)wiggle);
            var flightPath = ParentMap!.FindWorldRay(startWorldPosition, endWorldPosition);
            SnowballFightGame.SpawnSnowball(flightPath.Select(p => p.WorldPosition));
        }

        State = States.Idle;
        StateCompleteAction = null;
        targettedUnit = null;
    }
}
