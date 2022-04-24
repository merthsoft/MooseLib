namespace Merthsoft.Moose.Rays.Actors;

public static class BlinkingLightStates
{
    public const string Off = "Off";
    public const string On = "On";
}

public record BlinkingLightDef : ActorDef
{
    public BlinkingLightDef() : base("BlinkingLight", BlinkingLightStates.Off)
    {
        States = new()
        {
            { BlinkingLightStates.Off, new() { new(1, 100, Blocking: false, EndAction: BlinkingLight.CheckBlink) } },
            { BlinkingLightStates.On, new() { new(0, 100, Blocking: false, EndAction: BlinkingLight.CheckBlink) } }
        };
    }
}

public class BlinkingLight : Actor
{
    public BlinkingLight(BlinkingLightDef def, int x, int y) : base(def, x, y)
    {
    }

    public static void CheckBlink(Actor a)
    {
        var light = (a as BlinkingLight)!;
        var rand = RayGame.Instance.Random.NextSingle();
        if (rand < .5f)
            a.State = a.State == BlinkingLightStates.Off ? BlinkingLightStates.On : BlinkingLightStates.Off;
    }
}
