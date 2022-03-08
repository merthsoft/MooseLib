using Karcero.Engine;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;
public class DungeonMap : BaseMap
{
    public readonly DungeonLayer DungeonLayer;
    public readonly MonsterLayer MonsterLayer;

    public override int Height => DungeonLayer.Height;
    public override int Width => DungeonLayer.Width;
    public override int TileWidth => 16;
    public override int TileHeight => 16;
    public override IReadOnlyList<ILayer> Layers { get; }

    public DungeonMap(int width, int height)
    {
        Layers = new ILayer[]
        {
            DungeonLayer = new DungeonLayer(width, height),
            new ObjectLayer("player"),
            MonsterLayer = new MonsterLayer(),
            new ObjectLayer("spells"),
            new ObjectLayer("items"),
        };
    }

    public void GenerateRandomLevel()
    {
        DrawRoom(DungeonTile.StoneWall, DungeonTile.None, 0, 0, Width - 1, Height - 1);
        var generator = new DungeonGenerator<DungeonCell>();
        generator.AddMapProcessor(new DungeonMapProcessor());
        generator.GenerateA()
                 .DungeonOfSize(Width, Height)
                 .ABitRandom()
                 .WithBigChanceToRemoveDeadEnds()
                 .WithRoomSize(4, 20, 4, 20)
                 .SomewhatSparse()
                 .WithLargeNumberOfRooms()
                 .AndTellMeWhenItsDone(map =>
                 {
                     for (var x = 1; x < Width - 1; x++)
                         for (var y = 1; y < Height - 1; y++)
                         {
                             var t = map.GetCell(x - 1, y - 1);
                             SetDungeonTile(x, y, t.Tile);
                         }
                 });
    }

    public List<Rectangle> GenerateTown(int numRooms, (int, int)[] roomSizes)
    {
        FillDungeon(DungeonTile.None);
        DrawRoom(DungeonTile.StoneWall, DungeonTile.None, 0, 0, Width - 1, Height - 1);
        var rooms = GenerateRooms(numRooms, roomSizes);
        GenerateCorridors(rooms);
        GenerateDoors(rooms);
        HollowOutRooms(rooms);
        return rooms;
    }

    private void HollowOutRooms(List<Rectangle> rooms)
    {
        foreach (var (x, y, width, height) in rooms)
        {
            var randomFloor = (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.FLOOR_START, (int)DungeonTile.FLOOR_END);
            DrawRoom(GetDungeonTile(x, y), randomFloor, x, y, width, height);
        }
    }

    private List<Point> GenerateDoors(List<Rectangle> rooms)
    {
        var doors = new List<Point>();
        
        return doors;
    }

    private List<Point> GenerateCorridors(List<Rectangle> rooms)
    {
        var startingPoints = new List<Point>();
        var visitedPoints = new HashSet<Point>();
        while (true)
        {
            startingPoints.Clear();
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    if (GetDungeonTile(x, y) == DungeonTile.None && CountNeighbors(x, y) == 0)
                        startingPoints.Add(new(x, y));
                }

            if (startingPoints.Count == 0)
                return new List<Point>(visitedPoints);

            var randomFloor = (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.FLOOR_START, (int)DungeonTile.FLOOR_END);

            var startingPoint = startingPoints.RandomElement();

            var pointsToVisit = new Stack<Point>();
            pointsToVisit.Push(startingPoint);

            while (pointsToVisit.Count > 0 && pointsToVisit.Count < 1000)
            {
                var point = pointsToVisit.Pop();

                var (x, y) = point;

                visitedPoints.Add(point);

                var directions = new List<Point>();

                if (CountNeighbors(x - 1, y) == 0 && !visitedPoints.Contains(new(x - 1, y)))
                    directions.Add(new(x - 1, y));
                if (CountNeighbors(x + 1, y) == 0 && !visitedPoints.Contains(new(x + 1, y)))
                    directions.Add(new(x + 1, y));
                if (CountNeighbors(x, y - 1) == 0 && !visitedPoints.Contains(new(x, y - 1)))
                    directions.Add(new(x, y - 1));
                if (CountNeighbors(x, y + 1) == 0 && !visitedPoints.Contains(new(x, y + 1)))
                    directions.Add(new(x, y + 1));

                SetDungeonTile(x, y, randomFloor);

                while (directions.Count > 0)
                {
                    var next = MooseGame.Instance.Random.Next(directions.Count);
                    pointsToVisit.Push(directions[next]);
                    directions.RemoveAt(next);
                }
            }
        }
    }

    private int CountNeighbors(int x, int y, Point? lastPoint = null)
        => GetTileNormalized(lastPoint, x + 0, y + -1)
         + GetTileNormalized(lastPoint, x + -1, y + 0)
         + GetTileNormalized(lastPoint, x + 1, y + 0)
         + GetTileNormalized(lastPoint, x + 0, y + 1)
         + GetTileNormalized(lastPoint, x + -1, y + -1)
         + GetTileNormalized(lastPoint, x + -1, y + 1)
         + GetTileNormalized(lastPoint, x + 1, y + -1)
         + GetTileNormalized(lastPoint, x + -1, y + -1);

    private int GetTileNormalized(Point? lastPoint, int x, int y)
    {
        if (lastPoint != null && lastPoint?.X == x && lastPoint?.Y == y)
            return 0;

        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return 0;

        return GetDungeonTile(x, y) != DungeonTile.None ? 1 : 0;
    }

    private List<Rectangle> GenerateRooms(int numRooms, (int, int)[] roomSizes)
    {
        var rooms = new List<Rectangle>();
        for (var roomNumber = 0; roomNumber < numRooms; roomNumber++)
        {
            var (roomW, roomH) = (0, 0);
            var (x, y) = (0, 0);
            var roomRect = new Rectangle(0, 0, 0, 0);
            var numTries = 0;
            do
            {
                (roomW, roomH) = roomSizes.RandomElement();
                x = MooseGame.Instance.Random.Next(Width - roomW - 4) + 2;
                y = MooseGame.Instance.Random.Next(Height - roomH - 4) + 2;
                roomRect = new(x, y, roomW, roomH);
                numTries++;
                if (numTries > roomSizes.Length * 4)
                    break;
            } while (rooms.Any(r => r.Intersects(roomRect)));

            var randomWall = (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.WALL_START, (int)DungeonTile.WALL_END);
            DrawRoom(randomWall, randomWall, x, y, roomW, roomH);
            rooms.Add(new(x, y, roomW, roomH));
        }
        return rooms;
    }

    private void DrawRoom(DungeonTile wallTile, DungeonTile floorTile, int x, int y, int width, int height)
    {
        for (var i = 0; i <= width; i++)
            for (var j = 0; j <= height; j++)
                SetDungeonTile(i + x, j + y,
                    i == 0 || j == 0 || i == width || j == height ? wallTile : floorTile);
    }

    public void FillDungeon(DungeonTile tile)
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                SetDungeonTile(x, y, tile);
    }

    protected override int IsBlockedAt(int layer, int x, int y)
    {
        if (layer > 0)
            return 0;

        var tile = DungeonLayer.GetTileValue(x, y);
        return tile.IsBlocking() ? (int)tile : 0;
    }

    public void SetDungeonTile(int x, int y, DungeonTile t)
        => DungeonLayer.SetTileValue(x, y, t);

    public DungeonTile GetDungeonTile(int x, int y)
        => DungeonLayer.GetTileValue(x, y);
}
