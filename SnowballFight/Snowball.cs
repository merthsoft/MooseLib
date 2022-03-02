using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.SnowballFight;

public class Snowball : AnimatedGameObject
{
    public class States
    {
        public const string Fly = "fly";
        public const string Hit = "hit";
        public const string Dead = "dead";
    }

    public const string AnimationKey = "snowball";

    private Queue<Vector2> FlightPath { get; }
    private Vector2 StartCell { get; set; }

    public Snowball(IEnumerable<Vector2> flightPath)
        : base(SnowballFightGame.SnowballDef, flightPath.First(), SnowballFightGame.SnowballLayer, state: States.Fly)
    {
        FlightPath = new(flightPath.Where((v, i) => i % 3 == 0));

        if (FlightPath.Count == 0)
            State = States.Dead;

        SpriteTransform = new(new(-8, -8), 0, Vector2.One);
    }

    public override void OnAdd()
        => StartCell = GetCell();

    public override void Update(GameTime gameTime)
    {
        if (State == States.Fly)
        {
            Position = FlightPath.Dequeue();
            if (IsBlocked())
            {
                State = States.Hit;
                StateCompleteAction = () => State = States.Dead;
            }
        }

        if (State == States.Dead)
            Remove = true;

        base.Update(gameTime);
    }

    private bool IsBlocked()
        => FlightPath.Count == 0 || !ParentMap!.CellIsInBounds(GetCell())
        || (GetCell(ParentMap!) != StartCell
            && ParentMap!.GetBlockingVector(Position).Skip(2).Take(3).Any(b => b != 0));
}
