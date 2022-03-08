using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Dungeon;
public record SpellDef : AnimatedGameObjectDef
{
    public Texture2D Icon { get; private set; } = null!;

    public SpellDef(string spellDefName) : base(spellDefName, spellDefName)
    {
        
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        SpriteSheet = contentManager.LoadAnimatedSpriteSheet($"Spells/{DefName}/Animation", true, false);
        Icon = contentManager.Load<Texture2D>($"Spells/{DefName}/Icon");
    }
}
