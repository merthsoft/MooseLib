using Microsoft.Xna.Framework.Graphics;
using MooseLib;

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

        public UnitDef(string name, int maxHealth, int speed, float accuracySigma, Texture2D portrait, string? displayNameOverride = null)
        {
            AnimationKey = name;
            DefName = name;
            DisplayName = displayNameOverride ?? name.UpperFirst();
            MaxHealth = maxHealth;
            Speed = speed;
            AccuracySigma = accuracySigma;
            Portrait = portrait;
        }
    }
}
