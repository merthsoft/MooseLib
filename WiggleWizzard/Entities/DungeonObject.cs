using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.Dungeon.Map;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Entities;
public abstract record DungeonObjectDef : ManualAnimatedGameObjectDef
{
    public int HitPoints = 0;
    public int DrawIndex;
    public MiniMapTile MiniMapTile;

    public DungeonObjectDef(string DefName, string AnimationKey) : base(DefName, AnimationKey)
    {
        DefaultLayer = "dungeon";
        DefaultSize = new(16, 16);
    }
}

public abstract class DungeonObject : AnimatedGameObject
{
    public const string Alive = "alive";
    public const string Dead = "dead";
    public const string Dying = "dying";
    public const string Inanimate = "inanimate";

    protected WiggleWizzardGame game;
    protected DungeonPlayer player;

    public DungeonObjectDef DungeonObjectDef;

    public int HitPoints; 
    public int Armor = 0;
    public int MagicArmor = 0;

    public float AnimationRotation;
    public Vector2 AnimationPosition;

    public int DrawIndex;
    public MiniMapTile MiniMapTile;
    
    private List<Spell> activeSpells = new();
    public IEnumerable<Spell> ActiveSpells => activeSpells;

    public bool CurrentlyBlockingInput = false;

    public override string PlayKey => "idle";

    public new DungeonMap ParentMap { get; private set; }

    public DungeonObject(DungeonObjectDef def, Vector2? position, string direction, float? rotation, string layer)
        : base(def, position, layer, Vector2.Zero, rotation ?? 0, Vector2.One, "idle", direction)
    {
        game = WiggleWizzardGame.Instance;
        player = game.Player;

        DungeonObjectDef = def;
        HitPoints = def.HitPoints;
        State = HitPoints < 0 ? Inanimate : Alive;
        MiniMapTile = DungeonObjectDef.MiniMapTile;
        DrawIndex = def.DrawIndex;
        Origin = new(8, 8);
    }

    public override void SetMap(IMap map)
    {
        base.SetMap(map);
        ParentMap = (map as DungeonMap)!;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
        if (HitPoints <= 0 && State == Alive)
        {
            State = Dying;
            Die((game as WiggleWizzardGame)!);
            CurrentlyBlockingInput = true;
        }
        if (State == Dead)
        {
            Remove = true;
            CurrentlyBlockingInput = false;
        }
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
        => spriteBatch.Draw(Sprite.TextureRegion.Texture,
                WorldRectangle.Move(AnimationPosition).Move(Origin).ToRectangle(), 
                Sprite.TextureRegion.Bounds,
                Color, Rotation - AnimationRotation, Origin, SpriteEffects, LayerDepth);

    public virtual void Die(WiggleWizzardGame game)
    {
        this.AddTween(p => p.AnimationRotation, MathF.PI * 2, .25f, repeatCount: -1);
        this.AddTween(p => p.Scale, new Vector2(2, 2), .5f, 
            onEnd: _ => this.AddTween(p => p.Scale, new Vector2(0, 0), .25f,
                onEnd: _ => State = Dead));
    }

    public virtual void AddSpell(Spell spell)
        => activeSpells.Add(spell);

    public virtual void RemoveSpell(Spell spell)
        => activeSpells.Remove(spell);

    public virtual void TakeDamage(int value)
    {
        var totalLoss = value;
        while (MagicArmor > 0 && value > 0)
        {
            MagicArmor--;
            value--;
        }

        while (Armor > 0 && value > 0)
        {
            Armor--;
            value--;
        }

        while (HitPoints > 0 && value > 0)
        {
            HitPoints--;
            value--;
        }

        if (value > 0)
            game.SpawnFallingText($"OVERKILL", Position, Color.LightPink);

        game.SpawnFallingText($"-{totalLoss}", Position, Color.IndianRed);
        Color = Color.Red;
        ColorS = 1f;
        this.AddTween(p => p.ColorS, 0, .15f,
            onEnd: _ => this.AddTween(p => p.ColorS, 1, .15f, onEnd: _ => Color = Color.White));
    }

    public virtual void Heal(int value, bool overHeal = false)
    {
        HitPoints += value;
        var total = value;
        if (!overHeal && HitPoints > DungeonObjectDef.HitPoints)
        {
            total -= DungeonObjectDef.HitPoints - HitPoints;
            HitPoints = DungeonObjectDef.HitPoints;
        }
        game.SpawnFallingText(total.ToString(), Position, Color.Green);
    }
}