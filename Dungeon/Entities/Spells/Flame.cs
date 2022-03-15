namespace Merthsoft.Moose.Dungeon.Entities.Spells;
public record FlameDef : SpellDef { public FlameDef() : base("Flame", 5) { } }
public class Flame : Spell
{
    public Flame(SpellDef def, DungeonObject owner, Vector2 position) : base(def, owner, position)
    {
        Position = new Vector2((int)position.X / 16 * 16, (int)position.Y / 16 * 16);
        State = Cast;
        StateCompleteAction = () =>
        {
            State = Active;
            StateCompleteAction = null;
            CurrentlyBlockingInput = false;
        };
    }

    public override void Effect()
    {
        var (x, y) = GetCell();
        var target = game.GetMonster(x, y);
        target?.TakeDamage(5);
    }
}
