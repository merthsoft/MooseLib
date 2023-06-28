using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Spells;

public record FireballDef : SpellDef
{
    public FireballDef() : base("Fireball", 5)
    {
        
    }
}

public class Fireball : Spell
{
    IEnumerator<Vector2> FlightPath { get; }

    public Fireball(SpellDef def, DungeonObject owner, Vector2 end) 
        : base(def, owner, owner.Position)
    {
        Position = new Vector2((int)owner.Position.X / 16 * 16, (int)owner.Position.Y / 16 * 16);
        end = new((int)end.X / 16 * 16, (int)end.Y / 16 * 16);

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
        var (x, y) = GetCell();
        var target = game.GetMonster(x, y);
        target?.TakeDamage(3);
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
            var (x, y) = GetCell();
            if (game.GetDungeonTile(x, y).IsBlocking()
                || game.GetMonsterTile(x, y) != MonsterTile.None)
            {
                State = Hit;
                StateCompleteAction = () => State = Dead;
                ActiveTweens.ForEach(t => t.Cancel());
            }
        }

        base.Update(game, gameTime);
    }
}
