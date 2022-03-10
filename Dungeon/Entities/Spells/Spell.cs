using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon.Entities.Spells;

public abstract class Spell : AnimatedGameObject
{
    protected DungeonGame game;

    public new SpellDef Def => (SpellDef)base.Def;

    public const string Cast = "cast";
    public const string Active = "active";
    public const string Hit = "hit";
    public const string Dead = "dead";

    public DungeonObject Owner { get; }

    private bool hasHit = false;

    public Spell(SpellDef def, DungeonObject owner, Vector2 position) : base(def, position, "spells", state: Cast)
    {
        game = DungeonGame.Instance;
        Owner = owner;
        owner.AddSpell(this);
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
        if (State == Hit && !hasHit)
        {
            hasHit = true;
            Effect();
        }
        else if (State == Dead)
            Owner.RemoveSpell(this);
    }

    public abstract void Effect();
}
