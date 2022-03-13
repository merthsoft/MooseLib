using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon.Entities.Spells;

public record SpellDef : AnimatedGameObjectDef
{
    public Texture2D Icon { get; private set; } = null!;
    public string Name;
    public string Description = "A spell";

    public int ManaCost;

    public SpellDef(string spellDefName, int manaCost, string? name = null) : base(spellDefName, spellDefName)
    {
        DefaultLayer = "spells";
        DefaultOrigin = new(8, 8);
        DefaultScale = new(2f / 3f, 2f / 3f);
        Name = name ?? spellDefName;
        ManaCost = manaCost;
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        SpriteSheet = contentManager.LoadAnimatedSpriteSheet($"Spells/{DefName}/Animation", true, false);
        Icon = contentManager.Load<Texture2D>($"Spells/{DefName}/Icon");
    }

    public virtual void DrawExtraTargets(SpriteBatch spriteBatch) { }
}

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

    public virtual int ManaCost => Def.ManaCost;

    public Spell(SpellDef def, DungeonObject owner, Vector2 position) : base(def, position, "spells", state: Cast)
    {
        Position = new((int)Position.X / 16 * 16, (int)Position.Y / 16 * 16);
        game = DungeonGame.Instance;
        Owner = owner;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
        if (State == Hit && !hasHit)
        {
            hasHit = true;
            StateCompleteAction = () => State = Dead;
            Effect();
        }
        else if (State == Dead)
        {
            Owner.RemoveSpell(this);
            Remove = true;
        }
    }

    public abstract void Effect();
}
