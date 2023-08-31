using System.Xml.Serialization;

namespace Merthsoft.Moose.MooseEngine.PathFinding.Serialization;

public static class GridSerializer
{
    public static string SerializeGrid(Grid grid)
    {
        var gridDto = grid.ToDto();
        var xmlSerializer = new XmlSerializer(gridDto.GetType());

        using var textWriter = new StringWriter();
        xmlSerializer.Serialize(textWriter, gridDto);
        return textWriter.ToString();
    }

    public static Grid DeSerializeGrid(string gridString)
    {
        var xmlSerializer = new XmlSerializer(typeof(Merthsoft.Moose.MooseEngine.PathFinding.Serialization.GridDto));
        using var textReader = new StringReader(gridString);
        var gridDto = (Merthsoft.Moose.MooseEngine.PathFinding.Serialization.GridDto)xmlSerializer.Deserialize(textReader);
        var nodes = new Node[gridDto.Nodes.Length, gridDto.Nodes[0].Length];
        for (var i = 0; i < gridDto.Nodes.Length; i++)
            for (var j = 0; j < gridDto.Nodes[0].Length; j++)
            {
                var nodeDto = gridDto.Nodes[i][j];
                var node = new Node(nodeDto.Position.FromDto());
                nodes[i, j] = node;
            }

        for (var i = 0; i < gridDto.Nodes.Length; i++)
            for (var j = 0; j < gridDto.Nodes[0].Length; j++)
            {
                var nodeDto = gridDto.Nodes[i][j];
                var node = nodes[i, j];
                foreach (var outGoingEdge in nodeDto.OutGoingEdges)
                {
                    var toNode = nodes[outGoingEdge.End.X, outGoingEdge.End.Y];
                    throw new NotImplementedException("Need to figure out serialization");
                    //node.ConnectNode(toNode, outGoingEdge.TraversalVelocity.FromDto());
                }
            }

        return Grid.CreateGridFrom2DArrayOfNodes(nodes);
    }

    private static Merthsoft.Moose.MooseEngine.PathFinding.Serialization.GridDto ToDto(this Grid grid)
    {
        var nodeToGridPositionDict = new Dictionary<Node, Point>();
        var nodes = new NodeDto[grid.Columns][];
        for (var i = 0; i < grid.Columns; i++)
            for (var j = 0; j < grid.Rows; j++)
            {
                var gridPosition = new Point(i, j);
                nodeToGridPositionDict[grid.GetNode(gridPosition)] = gridPosition;
            }

        for (var i = 0; i < grid.Columns; i++)
        {
            nodes[i] = new NodeDto[grid.Rows];
            for (var j = 0; j < grid.Rows; j++)
                nodes[i][j] = grid.GetNode(new Point(i, j)).ToDto(nodeToGridPositionDict);
        }

        return new Merthsoft.Moose.MooseEngine.PathFinding.Serialization.GridDto
        {
            Nodes = nodes
        };
    }

    private static NodeDto ToDto(this Node node, Dictionary<Node, Point> nodeToGridPositionDict) => new NodeDto
    {
        Position = node.Position.ToPositionDto(),
        GridPoint = nodeToGridPositionDict[node].ToGridPoitionDto(),
        OutGoingEdges = node.Outgoing.ToDto(nodeToGridPositionDict),
        IncomingEdges = node.Incoming.ToDto(nodeToGridPositionDict),
    };

    private static List<EdgeDto> ToDto(this IList<Edge> edge, Dictionary<Node, Point> nodeToGridPositionDict) => edge.Select(e => e.ToDto(nodeToGridPositionDict)).ToList();

    private static EdgeDto ToDto(this Edge edge, Dictionary<Node, Point> nodeToGridPositionDict) => new EdgeDto
    {
        TraversalVelocity = edge.TraversalVelocity.ToDto(),
        Start = nodeToGridPositionDict[edge.Start].ToGridPoitionDto(),
        End = nodeToGridPositionDict[edge.End].ToGridPoitionDto()
    };

    private static Merthsoft.Moose.MooseEngine.PathFinding.Serialization.VelocityDto ToDto(this Velocity velocity) => new Merthsoft.Moose.MooseEngine.PathFinding.Serialization.VelocityDto
    {
        MetersPerSecond = velocity.MetersPerSecond
    };

    private static Velocity FromDto(this Merthsoft.Moose.MooseEngine.PathFinding.Serialization.VelocityDto velocity) => Velocity.FromMetersPerSecond(velocity.MetersPerSecond);

    private static Merthsoft.Moose.MooseEngine.PathFinding.Serialization.PositionDto ToPositionDto(this Vector2 position) => new Merthsoft.Moose.MooseEngine.PathFinding.Serialization.PositionDto
    {
        X = position.X,
        Y = position.Y
    };

    private static Vector2 FromDto(this Merthsoft.Moose.MooseEngine.PathFinding.Serialization.PositionDto position) => new Vector2(position.X, position.Y);

    private static GridPositionDto ToGridPoitionDto(this Point position) => new GridPositionDto
    {
        X = position.X,
        Y = position.Y
    };

    private static Point FromDto(this GridPositionDto position) => new Point(position.X, position.Y);
}