using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon.Entities;

public abstract class DungeonObject : TextureGameObject
{
    public const string Alive = "alive";
    public const string Dead = "dead";
    public const string Dying = "dying";
    public const string Inanimate = "inanimate";

    protected DungeonGame game;

    public DungeonObjectDef DungeonObjectDef { get; }

    public int HitPoints { get; set; }

    public float AnimationRotation { get; set; }
    public Vector2 AnimationPosition { get; set; }

    public abstract int DrawIndex { get; }
    
    private List<Spell> activeSpells = new();
    public IEnumerable<Spell> ActiveSpells => activeSpells;

    public bool CurrentlyBlockingInput { get; set; } = false;

    public DungeonObject(DungeonObjectDef def, Vector2? position, string direction, float? rotation, Vector2? size, string layer)
        : base(def, position, direction, rotation, size, layer)
    {
        game = DungeonGame.Instance;

        DungeonObjectDef = def;
        HitPoints = def.HitPoints;
        State = HitPoints < 0 ? Inanimate : Alive;
    }

    public override void Update(MooseGame game, GameTime gameTime)
    {
        base.Update(game, gameTime);
        if (HitPoints <= 0 && State == Alive)
        {
            State = Dying;
            Die((game as DungeonGame)!);
            CurrentlyBlockingInput = true;
        }
        if (State == Dead)
        {
            Remove = true;
            CurrentlyBlockingInput = false;
        }
    }

    public virtual void Die(DungeonGame game)
    {
        this.AddTween(p => p.AnimationRotation, MathF.PI * 2, .25f, repeatCount: -1);
        this.AddTween(p => p.Scale, new Vector2(2, 2), .5f, 
            onEnd: _ => this.AddTween(p => p.Scale, new Vector2(0, 0), .25f,
                onEnd: _ => State = Dead));
    }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
        => DrawSprite(spriteBatch, TextureGameObjectDef.Texture, DrawIndex);

    public void DrawSprite(SpriteBatch spriteBatch, Texture2D texture, int index)
    {
        if (index < 0)
            return;
        spriteBatch.Draw(texture,
            WorldRectangle.Move(AnimationPosition).Move(new(8, 8)).ToRectangle(), texture.GetSourceRectangle(index, 16, 16),
            Color, Rotation - AnimationRotation, new(8, 8), Effects, 1f);
    }

    public virtual void AddSpell(Spell spell)
        => activeSpells.Add(spell);

    public virtual void RemoveSpell(Spell spell)
        => activeSpells.Remove(spell);

    public virtual void TakeDamage(DungeonGame game, int value)
    {
        HitPoints -= value;
        game.SpawnFallingText($"-{value}", Position, Color.IndianRed);
        Color = Color.Red;
        ColorS = 1f;
        this.AddTween(p => p.ColorS, 0, .15f,
            onEnd: _ => this.AddTween(p => p.ColorS, 1, .15f, onEnd: _ => Color = Color.White));
    }

    public virtual void Heal(DungeonGame game, int value, bool overHeal = false)
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