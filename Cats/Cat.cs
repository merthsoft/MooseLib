using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;

namespace Merthsoft.Moose.Cats;
public record CatDef : AnimatedGameObjectDef
{
    public CatDef(string DefName) : base(DefName, DefName)
    {
    }

    public override void LoadContent(MooseContentManager contentManager)
        => SpriteSheet = contentManager.LoadAnimatedSpriteSheet($"Cats/{AnimationKey}/animation", usePrefix: false);
}
public class Cat : AnimatedGameObject
{
    public Cat(CatDef def, Vector2 position) : base(def, position, "cats", state: "idle")
    {
    }
}
