using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon.Entities.Spells;
public record SpellDef : AnimatedGameObjectDef
{
    public Texture2D Icon { get; private set; } = null!;
    public string Name;
    public string Description = "A spell";

    public int ManaCost;
    public TargetMode TargetMode;
    public bool BlocksPlayer = true;
    public bool TurnBased = false;

    public SpellDef(string spellDefName, int manaCost, string? displayName = null, TargetMode targetMode = TargetMode.Free) : base(spellDefName, spellDefName)
    {
        DefaultLayer = "spells";
        DefaultOrigin = new(8, 8);
        DefaultScale = new(2f / 3f, 2f / 3f);
        Name = displayName ?? spellDefName;
        ManaCost = manaCost;
        TargetMode = targetMode;
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
    protected WiggleWizzardGame game;
    protected DungeonPlayer player;

    public SpellDef SpellDef;

    public const string Cast = "cast";
    public const string Active = "active";
    public const string Hit = "hit";
    public const string Dead = "dead";

    public readonly DungeonObject Owner;

    public bool CurrentlyBlockingInput = true;
    protected bool hasHit = false;

    public virtual int ManaCost => SpellDef.ManaCost;
    
    protected int TurnsAlive = 2;

    public Spell(SpellDef def, DungeonObject owner, Vector2 position) : base(def, position, "spells", state: Cast)
    {
        SpellDef = def;
        Position = new((int)Position.X / 16 * 16, (int)Position.Y / 16 * 16);
        game = WiggleWizzardGame.Instance;
        player = DungeonPlayer.Instance;
        Owner = owner;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);

        if (SpellDef.TurnBased && !player.HasInputThisFrame && State != Dead)
            return;

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
