using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.SnowballFight;

public record UnitDef : AnimatedGameObjectDef
{
    public int MovementSpeed { get; }
    public int TurnSpeed { get; }
    public string DisplayName { get; }
    public int MaxHealth { get; }
    public float AccuracySigma { get; }

    public Texture2D Portrait { get; } = null!;

    public UnitDef(string name, int maxHealth, int movementSpeed, int turnSpeed, float accuracySigma, Texture2D portrait, string? displayNameOverride = null)
        : base(name, name)
    {
        DisplayName = displayNameOverride ?? name.UpperFirst();
        MaxHealth = maxHealth;
        MovementSpeed = movementSpeed;
        TurnSpeed = turnSpeed;
        AccuracySigma = accuracySigma;
        Portrait = portrait;
    }
}
