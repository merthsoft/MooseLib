namespace Merthsoft.Moose.Rays.Actors;

public record BlinkingLampDef : ActorDef
{
    public BlinkingLampDef() : base("BlinkingLight", BlinkingLightStates.Off)
    {
        States = new()
        {
            { BlinkingLightStates.Off, new() { new(1, 100, Blocking: true, EndAction: BlinkingLight.CheckBlink) } },
            { BlinkingLightStates.On, new() { new(0, 100, Blocking: true, EndAction: BlinkingLight.CheckBlink) } }
        };
    }
}