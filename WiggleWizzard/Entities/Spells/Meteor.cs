namespace Merthsoft.Moose.Dungeon.Entities.Spells;

public record MeteorDef : SpellDef
{
    public MeteorDef() : base("MeteorShower", 3, "Meteor")
    {
    }

    public override void DrawExtraTargets(SpriteBatch spriteBatch)
    {

    }
}

public class Meteor : Spell
{
    public Meteor(SpellDef def, DungeonObject owner, Vector2 position) 
        : base(def, owner, owner.Position)
    {
        Position = new Vector2((int)position.X / 16 * 16, (int)position.Y / 16 * 16);
        State = Active;
        StateCompleteAction = () => State = Hit;
    }

    public override void Effect()
    {
        var (x, y) = GetCell();
        var target = game.GetMonster(x, y);
        target?.TakeDamage(2);
    }

    public override void Update(MooseGame _, GameTime gameTime)
    {
        if (State == Dead)
            Remove = true;

        base.Update(game, gameTime);
    }
}
