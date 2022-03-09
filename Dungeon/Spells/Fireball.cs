using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon.Spells;

public record FireballDef : SpellDef
{
    public FireballDef() : base("Fireball")
    {
        DefaultOrigin = new(8,8);
        DefaultSize = new(24, 24);
        DefaultScale = new(2f / 3f, 2f / 3f);
    }
}

public class Fireball : Spell
{
    IEnumerator<Vector2> FlightPath { get; }

    public Fireball(SpellDef def, GameObjectBase owner, Vector2 end) 
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

    public override void Update(MooseGame game, GameTime gameTime)
    {
        var dungeonGame = (game as DungeonGame)!;
        if (State == Active)
        {
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            FlightPath.MoveNext();
            FlightPath.MoveNext();

            Position = FlightPath.Current;
            var (x, y) = GetCell();
            if (dungeonGame.GetDungeonTile(x, y).IsBlocking())
            {
                State = Hit;
                ActiveTweens.ForEach(t => t.Cancel());
                StateCompleteAction = () => State = Dead;
            }
        }

        if (State == Dead)
            Remove = true;

        base.Update(game, gameTime);
    }
}
