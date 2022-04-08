namespace Merthsoft.Moose.Rays;

public delegate TColor RayShader<TColor>(int x, int y, double distance, double viewAngleDegrees, TColor color);
