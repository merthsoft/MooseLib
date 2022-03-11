﻿namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ILayer
{
    string Name { get; }
    bool IsHidden { get; set; }
    Vector2 DrawOffset { get; set; }
    Vector2 DrawSize { get; set; }

    string? RendererKey { get; }
}
