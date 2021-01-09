using MonoGame.Extended.Sprites;

namespace MooseLib.Defs
{
    public record AnimatedGameObjectDef : GameObjectDef
    {
        public string AnimationKey { get; init; }
        public SpriteSheet? SpriteSheet { get; set; }

        public AnimatedGameObjectDef(string defName, string animationKey) : base(defName)
            => AnimationKey = animationKey;

        public override void LoadContent(MooseContentManager contentManager)
            => SpriteSheet = contentManager.LoadAnimatedSpriteSheet(AnimationKey);
    }
}
