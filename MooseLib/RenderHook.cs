namespace Merthsoft.Moose.MooseEngine;

public record RenderHook(Action<int>? PreHook = null, Action<int>? PostHook = null)
{

}
