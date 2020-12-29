using Microsoft.Xna.Framework.Graphics;

namespace SnowballFight
{
    public record UnitDef
    {
        public string DefName { get; init; } = "";
        public string AnimationKey { get; init; } = "";
        public int Speed { get; init; }
        public string DisplayName { get; init; } = "";
        public int MaxHealth { get; init; }
        public float AccuracySigma { get; init; }
        public Texture2D Portrait { get; init; } = null!;
    }
}
