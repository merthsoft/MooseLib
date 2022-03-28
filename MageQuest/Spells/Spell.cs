using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.MageQuest.Spells;
public record SpellDef : AnimatedGameObjectDef
{
    public Texture2D Icon { get; private set; } = null!;
    public string Name;
    public string Description = "A spell";

    public int ManaCost;

    public SpellDef(string spellDefName, int manaCost, string? displayName = null) : base(spellDefName, spellDefName)
    {
        DefaultLayer = "spells";
        DefaultOrigin = new(12, 12);
        DefaultSize = new(24, 24);
        Name = displayName ?? spellDefName;
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
    public SpellDef SpellDef;

    public const string Cast = "cast";
    public const string Active = "active";
    public const string Hit = "hit";
    public const string Dead = "dead";

    public bool CurrentlyBlockingInput = true;
    protected bool hasHit = false;

    public virtual int ManaCost => SpellDef.ManaCost;

    protected int TurnsAlive = 2;

    public Spell(SpellDef def, Vector2 position) : base(def, position, "spells", state: Cast)
    {
        SpellDef = def;
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
            Remove = true;
    }

    public abstract void Effect();
}
