using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace Merthsoft.Moose.MooseEngine.Defs
{
    public record AnimatedGameObjectDef : GameObjectDef
    {
        public string AnimationKey { get; set; }
        public Vector2 Origin { get; set; } = Vector2.Zero;
        
        public SpriteSheet? SpriteSheet { get; private set; }

        public AnimatedGameObjectDef(string defName, string animationKey) : base(defName)
            => AnimationKey = animationKey;

        public override void LoadContent(MooseContentManager contentManager)
            => SpriteSheet = contentManager.LoadAnimatedSpriteSheet(AnimationKey);
    }
}
