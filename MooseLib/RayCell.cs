namespace Merthsoft.Moose.MooseEngine;

public record RayCell(Vector2 WorldPosition, IList<int> BlockedVector);
