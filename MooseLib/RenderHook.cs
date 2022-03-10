namespace Merthsoft.Moose.MooseEngine;

public record RenderHook(Action<string>? PreHook = null, Action<string>? PostHook = null)
{

}
