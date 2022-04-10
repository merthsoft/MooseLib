using MonoGame.Extended.Sprites;

namespace Merthsoft.Moose.MooseEngine.Defs;
public record AnimatedGameObjectDef(string DefName, string AnimationKey) : GameObjectDef(DefName)
{
    public SpriteSheet SpriteSheet { get; protected set; } = null!;

    public override void LoadContent(MooseContentManager contentManager)
        => SpriteSheet = contentManager.LoadAnimatedSpriteSheet(AnimationKey);
}
