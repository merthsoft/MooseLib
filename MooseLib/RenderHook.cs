namespace Merthsoft.MooseEngine
{
    public record RenderHook(Action<int>? PreHook = null, Action<int>? PostHook = null)
    {
        
    }
}
