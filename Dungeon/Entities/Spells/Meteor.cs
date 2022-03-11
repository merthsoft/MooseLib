namespace Merthsoft.Moose.Dungeon.Entities.Spells;

public record MeteorDef : SpellDef
{
    public MeteorDef() : base("MeteorShower", "Meteor")
    {
    }

    public override void DrawExtraTargets(SpriteBatch spriteBatch)
    {

    }
}

public class Meteor : Spell
{
    public Meteor(SpellDef def, DungeonObject owner, Vector2 end) 
        : base(def, owner, owner.Position)
    {
        Position = end;
        State = Active;
        StateCompleteAction = () => State = Hit;
    }

    public override void Effect()
    {
        var (x, y) = GetCell();
        var target = game.GetMonster(x, y);
        target?.TakeDamage(game, 2);
    }

    public override void Update(MooseGame _, GameTime gameTime)
    {
        if (State == Dead)
            Remove = true;

        base.Update(game, gameTime);
    }
}
