using Microsoft.Xna.Framework.Graphics;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.SnowballFight
{
    public record UnitDef : AnimatedGameObjectDef
    {
        public int Speed { get; }
        public string DisplayName { get; }
        public int MaxHealth { get; }
        public float AccuracySigma { get; }

        public Texture2D Portrait { get; } = null!;

        public UnitDef(string name, int maxHealth, int speed, float accuracySigma, Texture2D portrait, string? displayNameOverride = null)
            : base(name, name)
        {
            DisplayName = displayNameOverride ?? name.UpperFirst();
            MaxHealth = maxHealth;
            Speed = speed;
            AccuracySigma = accuracySigma;
            Portrait = portrait;
        }
    }
}
