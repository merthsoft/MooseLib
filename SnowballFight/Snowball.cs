using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.Tiled;

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

    private IEnumerator<Vector2> FlightPath { get; }
    private Point StartCell { get; set; }

    public new TiledMooseMap ParentMap { get; private set; }

    public Snowball(IEnumerable<Vector2> flightPath)
        : base(SnowballFightGame.SnowballDef, flightPath.First(), SnowballFightGame.SnowballLayer, state: States.Fly)
    {
        FlightPath = flightPath.GetEnumerator();

        if (!FlightPath.MoveNext())
            State = States.Dead;

        DrawOffset = new(-8, -8);
    }

    public override void OnAdd()
    {
        base.OnAdd();
        StartCell = Cell;
        ParentMap = (base.ParentMap as TiledMooseMap)!;
    }

    public override void Update(MooseGame _game, GameTime gameTime)
    {
        if (State == States.Fly)
        {
            Position = FlightPath.Current;
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            if (IsBlocked())
            {
                State = States.Hit;
                StateCompleteAction = () => State = States.Dead;
            }
        }

        if (State == States.Dead)
            Remove = true;

        base.Update(_game, gameTime);
    }

    private bool IsBlocked()
        => !ParentMap.CellIsInBounds(Cell)
        || (Cell != StartCell && ParentMap.GetBlockingVector(Position).Skip(2).Take(3).Any(b => b != 0));
}
