namespace Merthsoft.Moose.MageQuest.Spells;

public record FireballDef : SpellDef
{
    public FireballDef() : base("Fireball", 5)
    {

    }
}

public class Fireball : Spell
{
    IEnumerator<Vector2> FlightPath { get; }

    public Fireball(SpellDef def, Vector2 start, Vector2 end)
        : base(def, start)
    {
        var (x1, y1) = Position;
        var (x2, y2) = end;

        var xDiff = x2 - x1;
        var yDiff = y2 - y1;
        Rotation = MathF.Atan2(yDiff, xDiff);

        StateCompleteAction = () => State = Active;

        FlightPath = Position.CastRay(end, true, true, true, true).GetEnumerator();
    }

    public override void Effect()
    {
        
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        if (State == Active)
        {
            StateCompleteAction = null;
            for (var i = 0; i < 5; i++)
            {
                FlightPath.MoveNext();

                Position = FlightPath.Current;
                if (MageGame.Instance.BlocksPlayer(Position))
                {
                    State = Hit;
                    StateCompleteAction = () => State = Dead;
                    ActiveTweens.ForEach(t => t.Cancel());
                    break;
                }
            }
        }

        base.Update(game, gameTime);
    }
}
