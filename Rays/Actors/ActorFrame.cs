namespace Merthsoft.Moose.Rays.Actors;

public record ActorFrame(int FrameOffset = 0, float Length = 0,
    bool Shootable = false, bool Blocking = true, string? NextState = null, ObjectRenderMode? RenderMode = null,
    Action<Actor>? StartAction = null, Action<Actor>? EndAction = null)
{
}
