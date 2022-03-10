namespace Merthsoft.Moose.MooseEngine;

public record StartupParameters
{
    public BlendState? BlendState { get; set; }
    public SamplerState? SamplerState { get; set; }
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public bool Fullscreen { get; set; }
    public int? RandomSeed { get; set; }
    public int StateStackSize { get; set; }
    public Rectangle? CameraRectangle { get; set; }
    public Color DefaultBackgroundColor { get; set; }
}
