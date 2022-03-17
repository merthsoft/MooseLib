using Merthsoft.Moose.Dungeon.Entities.Items;
using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Dungeon.Entities;
public abstract record DungeonObjectDef : TextureGameObjectDef
{
    public int HitPoints = 0;
    public int DrawIndex;
    public MiniMapTile MiniMapTile;

    public DungeonObjectDef(string DefName, string TextureName, Rectangle? SourceRectangle = null) : base(DefName, TextureName, SourceRectangle)
    {
        DefaultLayer = "dungeon";
        DefaultSize = new(16, 16);
    }
}
public abstract class DungeonObject : TextureGameObject
{
    public const string Alive = "alive";
    public const string Dead = "dead";
    public const string Dying = "dying";
    public const string Inanimate = "inanimate";

    protected DungeonGame game;
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

    public DungeonObject(DungeonObjectDef def, Vector2? position, string direction, float? rotation, Vector2? size, string layer)
        : base(def, position, direction, rotation, size, layer)
    {
        game = DungeonGame.Instance;
        player = game.Player;

        DungeonObjectDef = def;
        HitPoints = def.HitPoints;
        State = HitPoints < 0 ? Inanimate : Alive;
        MiniMapTile = DungeonObjectDef.MiniMapTile;
        DrawIndex = def.DrawIndex;
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
        => DrawSprite(spriteBatch, TextureGameObjectDef.Texture, DrawIndex, this is Chest ? 1 : .5f);

    public void DrawSprite(SpriteBatch spriteBatch, Texture2D texture, int index, float layerDepth)
    {
        if (index < 0)
            return;
        spriteBatch.Draw(texture,
            WorldRectangle.Move(new(AnimationPosition.X + 8, AnimationPosition.Y + 8)).ToRectangle(), 
            texture.GetSourceRectangle(index, 16, 16),
            Color, Rotation - AnimationRotation, new(8, 8), Effects, layerDepth);
    }

    public virtual void AddSpell(Spell spell)
        => activeSpells.Add(spell);

    public virtual void RemoveSpell(Spell spell)
        => activeSpells.Remove(spell);

    public virtual void TakeDamage(int value)
    {
        var totalLoss = 0;
        while (MagicArmor > 0 && value > 0)
        {
            MagicArmor--;
            value--;
            totalLoss++;
        }

        while (Armor > 0 && value > 0)
        {
            Armor--;
            value--;
            totalLoss++;
        }

        while (HitPoints > 0 && value > 0)
        {
            HitPoints--;
            value--;
            totalLoss++;
        }

        if (value > 0)
            game.SpawnFallingText($"OVERKILL", Position, Color.DarkRed);

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