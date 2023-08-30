namespace Merthsoft.Moose.MooseEngine.PathFinding.Grids;

public sealed class Grid
{
    public readonly Node[,] Nodes;

    public static Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid CreateGridWithLateralConnections(GridSize gridSize, Size cellSize, Velocity traversalVelocity)
    {
        CheckArguments(gridSize, cellSize, traversalVelocity);

        var grid = new Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid(gridSize, cellSize);

        grid.CreateLateralConnections(traversalVelocity);

        return grid;
    }

    public static Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid CreateGridWithDiagonalConnections(GridSize gridSize, Size cellSize, Velocity traversalVelocity)
    {
        CheckArguments(gridSize, cellSize, traversalVelocity);

        var grid = new Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid(gridSize, cellSize);

        grid.CreateDiagonalConnections(traversalVelocity);

        return grid;
    }

    public static Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid CreateGridWithLateralAndDiagonalConnections(GridSize gridSize, Size cellSize, Velocity traversalVelocity)
    {
        CheckArguments(gridSize, cellSize, traversalVelocity);

        var grid = new Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid(gridSize, cellSize);

        grid.CreateDiagonalConnections(traversalVelocity);
        grid.CreateLateralConnections(traversalVelocity);

        return grid;
    }

    public static Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid CreateGridFrom2DArrayOfNodes(Node[,] nodes) => new Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid(nodes);

    private static void CheckGridSize(GridSize gridSize)
    {
        if (gridSize.Columns < 1)
            throw new ArgumentOutOfRangeException(
                nameof(gridSize), $"Argument {nameof(gridSize.Columns)} is {gridSize.Columns} but should be >= 1");

        if (gridSize.Rows < 1)
            throw new ArgumentOutOfRangeException(
                nameof(gridSize), $"Argument {nameof(gridSize.Rows)} is {gridSize.Rows} but should be >= 1");
    }

    private static void CheckArguments(GridSize gridSize, Size cellSize, Velocity defaultSpeed)
    {
        CheckGridSize(gridSize);


        if (cellSize.Width <= Distance.Zero)
            throw new ArgumentOutOfRangeException(
                nameof(cellSize), $"Argument {nameof(cellSize.Width)} is {cellSize.Width} but should be > {Distance.Zero}");

        if (cellSize.Height <= Distance.Zero)
            throw new ArgumentOutOfRangeException(
                nameof(cellSize), $"Argument {nameof(cellSize.Height)} is {cellSize.Height} but should be > {Distance.Zero}");

        if (defaultSpeed.MetersPerSecond <= 0.0f)
            throw new ArgumentOutOfRangeException(
                nameof(defaultSpeed), $"Argument {nameof(defaultSpeed)} is {defaultSpeed} but should be > 0.0 m/s");
    }

    private Grid(Node[,] nodes)
    {
        GridSize = new GridSize(nodes.GetLength(0), nodes.GetLength(1));
        CheckGridSize(GridSize);
        Nodes = nodes;
    }

    private Grid(GridSize gridSize, Size cellSize)
    {
        GridSize = gridSize;
        Nodes = new Node[gridSize.Columns, gridSize.Rows];

        CreateNodes(cellSize);
    }

    private void CreateNodes(Size cellSize)
    {
        for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
                Nodes[x, y] = new Node((cellSize.Width * x).Meters, (cellSize.Height * y).Meters);
    }

    private void CreateLateralConnections(Velocity defaultSpeed)
    {
        for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
            {
                var node = Nodes[x, y];

                if (x < Columns - 1)
                {
                    var eastNode = Nodes[x + 1, y];
                    ConnectNodes(node, eastNode, defaultSpeed);
                }

                if (y < Rows - 1)
                {
                    var southNode = Nodes[x, y + 1];
                    ConnectNodes(node, southNode, defaultSpeed);
                }
            }
    }

    private static void ConnectNodes(Node node, Node eastNode, Velocity defaultSpeed)
    {
        node.ConnectIncoming(eastNode, defaultSpeed);
        node.ConnectOutgoing(eastNode, defaultSpeed);
        eastNode.ConnectIncoming(node, defaultSpeed);
        eastNode.ConnectOutgoing(node, defaultSpeed);
    }

    private void CreateDiagonalConnections(Velocity defaultSpeed)
    {
        for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
            {
                var node = Nodes[x, y];

                if (x < Columns - 1 && y < Rows - 1)
                {
                    var southEastNode = Nodes[x + 1, y + 1];
                    ConnectNodes(node, southEastNode, defaultSpeed);
                }

                if (x > 0 && y < Rows - 1)
                {
                    var southWestNode = Nodes[x - 1, y + 1];
                    ConnectNodes(node, southWestNode, defaultSpeed);
                }
            }
    }

    public GridSize GridSize { get; }

    public int Columns => GridSize.Columns;

    public int Rows => GridSize.Rows;

    public INode GetNode(Point  position) => Nodes[position.X, position.Y];
    public INode GetNode(int x, int y) => Nodes[x, y];

    public IReadOnlyList<INode> GetAllNodes()
    {
        var list = new List<INode>(Columns * Rows);

        for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
                list.Add(Nodes[x, y]);

        return list;
    }

    public Merthsoft.Moose.MooseEngine.PathFinding.Grids.Grid DisconnectIncomingWhere(Func<int, int, bool> func)
    {
        for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
                if (func(x, y))
                    DisconnectIncoming(x, y);

        return this;
    }

    public void DisconnectNode(int x, int y)
    {
        var node = Nodes[x, y];
        node.DisconnectAll();
    }

    public void DisconnectIncoming(int x, int y)
    {
        var node = Nodes[x, y];
        node.DisconnectIncoming();
    }

    public void ConnectIncomingLaterally(int x, int y, Velocity velocity)
    {
        Point position = new(x, y);

        var left = new Point (x - 1, y);
        if (IsInsideGrid(left))
            ConnectIncomming(position, left, velocity);

        var top = new Point (x, y - 1);
        if (IsInsideGrid(top))
            ConnectIncomming(position, top, velocity);

        var right = new Point (x + 1, y);
        if (IsInsideGrid(right))
            ConnectIncomming(position, right, velocity);

        var bottom = new Point (x, y + 1);
        if (IsInsideGrid(bottom))
            ConnectIncomming(position, bottom, velocity);
    }

    public void RemoveDiagonalConnectionsIntersectingWithNode(Point  position)
    {
        var left = new Point (position.X - 1, position.Y);
        var top = new Point (position.X, position.Y - 1);
        var right = new Point (position.X + 1, position.Y);
        var bottom = new Point (position.X, position.Y + 1);

        if (IsInsideGrid(left) && IsInsideGrid(top))
        {
            RemoveEdge(left, top);
            RemoveEdge(top, left);
        }

        if (IsInsideGrid(top) && IsInsideGrid(right))
        {
            RemoveEdge(top, right);
            RemoveEdge(right, top);
        }

        if (IsInsideGrid(right) && IsInsideGrid(bottom))
        {
            RemoveEdge(right, bottom);
            RemoveEdge(bottom, right);
        }

        if (IsInsideGrid(bottom) && IsInsideGrid(left))
        {
            RemoveEdge(bottom, left);
            RemoveEdge(left, bottom);
        }
    }

    public void RemoveEdge(Point  from, Point  to)
    {
        var fromNode = Nodes[from.X, from.Y];
        var toNode = Nodes[to.X, to.Y];

        fromNode.Disconnect(toNode);
    }

    public void ConnectIncomming(Point  from, Point  to, Velocity traversalVelocity)
    {
        var fromNode = Nodes[from.X, from.Y];
        var toNode = Nodes[to.X, to.Y];

        fromNode.ConnectIncoming(toNode, traversalVelocity);
    }

    public void ConnectOutgoing(int startX, int endX, Velocity defaultVelocity) => throw new System.NotImplementedException();
    private bool IsInsideGrid(Point  position) => position.X >= 0 && position.X < Columns && position.Y >= 0 && position.Y < Rows;
}
