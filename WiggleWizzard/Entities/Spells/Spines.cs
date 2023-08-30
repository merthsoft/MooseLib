namespace Merthsoft.Moose.Dungeon.Entities.Spells;

public record SpinesDef : SpellDef
{
    public SpinesDef() : base("Earth Shard", 2, "Spines")
    {
    }

    public override void DrawExtraTargets(SpriteBatch spriteBatch)
    {

    }
}

public class Spines : Spell
{
    readonly IEnumerator<Vector2> FlightPath;
    readonly HashSet<DungeonObject> HitObjects = new();

    public Spines(SpellDef def, DungeonObject owner, Vector2 end)
        : base(def, owner, owner.Position)
    {
        Position = new Vector2((int)owner.Position.X / 16 * 16 + 8, (int)owner.Position.Y / 16 * 16 + 8);
        end = new((int)end.X / 16 * 16 + 8, (int)end.Y / 16 * 16 + 8);

        var (x1, y1) = Position;
        var (x2, y2) = end;

        float xDiff = x2 - x1;
        float yDiff = y2 - y1;
        Rotation = MathF.Atan2(yDiff, xDiff);

        StateCompleteAction = () => State = Active;

        FlightPath = Position.CastRay(end, true, true, true, true).GetEnumerator();
    }

    public override void Effect()
    {
        var (x, y) = Cell;
        var target = game.GetMonster(x, y);
        if (target != null && !HitObjects.Contains(target))
        {
            HitObjects.Add(target);
            target.TakeDamage(1);
        }
    }

    public override void Update(MooseGame _, GameTime gameTime)
    {
        if (State == Active)
        {
            StateCompleteAction = null;
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            FlightPath.MoveNext();

            Position = FlightPath.Current;
            var (x, y) = Cell;
            var monster = game.GetMonster(x, y);
            if (game.GetDungeonTile(x, y).IsBlocking())
            {
                State = Hit;
                StateCompleteAction = () => State = Dead;
                ActiveTweens.ForEach(t => t.Cancel());
            } else if (monster != null)
                Effect();
        }

        base.Update(game, gameTime);
    }
}
