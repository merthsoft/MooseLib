namespace Merthsoft.Moose.MooseEngine.PathFinding.Paths;

public interface IPathFinder
{
    Path FindPath(INode start, INode goal, Velocity maximumVelocity);

    Path FindPath(int x1, int y1, int x2, int y2, Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid grid)
    {
        var startNode = grid.GetNode(x1, y1);
        var endNode = grid.GetNode(x2, y2);

        var maximumVelocity = grid.GetAllNodes().SelectMany(n => n.Outgoing).Where(e => e.IsConnected).Select(e => e.TraversalVelocity).Max();

        return FindPath(startNode, endNode, maximumVelocity);
    }

    Path FindPath(int x1, int y1, int x2, int y2, Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid grid, Velocity maximumVelocity)
    {
        var startNode = grid.GetNode(x1, y1);
        var endNode = grid.GetNode(x2, y2);

        return FindPath(startNode, endNode, maximumVelocity);
    }
}
