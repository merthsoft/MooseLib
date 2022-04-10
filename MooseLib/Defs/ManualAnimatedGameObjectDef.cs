namespace Merthsoft.Moose.MooseEngine.Defs;

public record ManualAnimatedGameObjectDef(string DefName, string AnimationKey) : AnimatedGameObjectDef(DefName, AnimationKey)
{
    public override void LoadContent(MooseContentManager contentManager) { }
}
