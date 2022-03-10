using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon.Entities.Spells;
public record SpellDef : AnimatedGameObjectDef
{
    public Texture2D Icon { get; private set; } = null!;
    public string Name { get; set; }

    public SpellDef(string spellDefName, string? name = null) : base(spellDefName, spellDefName)
    {
        DefaultLayer = "spells";
        DefaultOrigin = new(8, 8);
        DefaultSize = new(24, 24);
        DefaultScale = new(2f / 3f, 2f / 3f);
        Name = name ?? spellDefName;
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        SpriteSheet = contentManager.LoadAnimatedSpriteSheet($"Spells/{DefName}/Animation", true, false);
        Icon = contentManager.Load<Texture2D>($"Spells/{DefName}/Icon");
    }
}
