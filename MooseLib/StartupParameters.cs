﻿namespace Merthsoft.Moose.MooseEngine;

public record StartupParameters
{
    public BlendState? BlendState { get; set; }
    public SamplerState? SamplerState { get; set; }
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public bool IsFullscreen { get; set; }
    public int? RandomSeed { get; set; }
    public int StateStackSize { get; set; }
    public Rectangle? CameraRectangle { get; set; }
    public Color DefaultBackgroundColor { get; set; }
    public bool IsMouseVisible { get; set; }
    public RenderMode RenderMode { get; set; }
    public Action<MooseGame, Exception>? UpdateExceptionHook { get; set; }
    public Action<MooseGame, Exception>? DrawExceptionHook { get; set; }
}
