﻿namespace Merthsoft.Moose.MooseEngine.PathFinding;

public interface IPathFinder
{
    Path FindPath(int x1, int y1, int x2, int y2, Grid grid, Velocity? maximumVelocity = null);
}
