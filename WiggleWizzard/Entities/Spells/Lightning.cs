namespace Merthsoft.Moose.Dungeon.Entities.Spells;
public record LightningDef : SpellDef
{
    public LightningDef() : base("Lightning", 5)
    {
    }

    public override void DrawExtraTargets(SpriteBatch spriteBatch)
    {

    }
}

public class Lightning : Spell
{
    public int Damage = 3;
    public Lightning(SpellDef def, DungeonObject owner, Vector2 position) : base(def, owner, position)
    {
        Position = new Vector2((int)position.X / 16 * 16, (int)position.Y / 16 * 16);
        State = Active;
        StateCompleteAction = () =>
        {
            State = Hit;
            StateCompleteAction = () => State = Dead;
        };
    }

    public override void Effect()
    {
        var (x, y) = Cell;
        var target = game.GetMonster(x, y);
        target?.TakeDamage(Damage);
    }
}
