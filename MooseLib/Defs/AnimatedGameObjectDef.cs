using MonoGame.Extended.Sprites;

namespace MooseLib.Defs
{
    public record AnimatedGameObjectDef : GameObjectDef
    {
        public string AnimationKey { get; init; }
        public SpriteSheet? SpriteSheet { get; set; }

        public AnimatedGameObjectDef(string animationKey) => AnimationKey = animationKey;
    }
}
