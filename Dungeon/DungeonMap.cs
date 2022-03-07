using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Promethean.Core;

namespace Merthsoft.Moose.Dungeon;
internal class DungeonMap : BaseMap
{
    public readonly DungeonLayer DungeonLayer;

    public override int Height => DungeonLayer.Height;
    public override int Width => DungeonLayer.Width;
    public override int TileWidth => 16;
    public override int TileHeight => 16;
    public override IReadOnlyList<ILayer> Layers { get; }

    public DungeonMap(int width, int height)
    {
        Layers = new ILayer[]
        {
            DungeonLayer = new DungeonLayer("dungeon", width, height),
            new ObjectLayer("player"),
        };
    }

    void GenerateRandomLevel()
    {
        var generator = new LevelGenerator(options);
        var level = generator.Generate();
        var renderedLevel = level.Render();
        var xLength = tile.GetComponent<Renderer>().bounds.size.x;
        var zLength = tile.GetComponent<Renderer>().bounds.size.z;

        for (var x = 0; x < renderedLevel.GetLength(0); x++)
        {
            for (var y = 0; y < renderedLevel.GetLength(1); y++)
            {
                var value = renderedLevel[x, y];
                if (value == Tile.Empty)
                {
                    continue;
                }
                else
                {
                    var currentTile = Instantiate(tile, new Vector3(xLength * x, 0, zLength * y), new Quaternion());

                }
            }
        }
    }


    public void GenerateDungeon(int numRooms, (int, int)[] roomSizes)
    {
        FillDungeon(Tile.None);
        DrawRoom(Tile.StoneWall, Tile.None, 0, 0, Width - 1, Height - 1);
        Rooms = GenerateRooms(numRooms, roomSizes);
        Coridors = GenerateCorridors();
        Doors = GenerateDoors(true);
        HollowOutRooms();
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                if (GetDungeonTile(x, y) == Tile.None)
                    SetDungeonTile(x, y, Tile.StoneWall);
            }
    }

    public void GenerateTown(int numRooms, (int, int)[] roomSizes)
    {
        FillDungeon(Tile.None);
        DrawRoom(Tile.StoneWall, Tile.None, 0, 0, Width - 1, Height - 1);
        Rooms = GenerateRooms(numRooms, roomSizes);
        Coridors = GenerateCorridors();
        Doors = GenerateDoors(false);
        HollowOutRooms();
    }

    private void HollowOutRooms()
    {
        foreach (var (x, y, width, height) in Rooms)
        {
            var randomFloor = (Tile)MooseGame.Instance.Random.Next((int)Tile.FLOOR_START, (int)Tile.FLOOR_END);
            DrawRoom(GetDungeonTile(x, y), randomFloor, x, y, width, height);
        }
    }

    private List<Point> GenerateDoors(bool connectCorridors)
    {
        var doors = new List<Point>();
        for (var y = 2; y < Height - 2; y++)
        {
            var potentialDoorGroup = new List<Point>();
            for (var x = 2; x < Width - 2; x++)
            {
                if (GetDungeonTile(x, y) != Tile.None)
                    continue;
                var north = GetDungeonTile(x, y - 1);
                var south = GetDungeonTile(x, y + 1);
                var east = GetDungeonTile(x - 1, y);
                var west = GetDungeonTile(x + 1, y);

                if (
                    north.IsWall() && south.IsWall() ||
                    east.IsWall() && west.IsWall() ||
                    north.IsWall() && south.IsFloor() ||
                    north.IsFloor() && south.IsWall() ||
                    east.IsWall() && west.IsFloor() ||
                    east.IsFloor() && west.IsWall() ||
                    (connectCorridors && (
                        north.IsFloor() && south.IsFloor() ||
                        east.IsFloor() && west.IsFloor()
                    )))
                    potentialDoorGroup.Add(new(x, y));
            }
            if (potentialDoorGroup.Count != 0)
            {
                var door = potentialDoorGroup.RemoveRandomElement();
                var (doorX, doorY) = door;

                var north = GetDungeonTile(doorX, doorY - 1);
                var south = GetDungeonTile(doorX, doorY + 1);
                var east = GetDungeonTile(doorX - 1, doorY);
                var west = GetDungeonTile(doorX + 1, doorY);

                if (north.IsDoor() || south.IsDoor() || east.IsDoor() || west.IsDoor())
                    continue;

                var randomDoor = (Tile)MooseGame.Instance.Random.Next((int)Tile.DOOR_START, (int)Tile.DOOR_END);
                SetDungeonTile(doorX, doorY, randomDoor);
                doors.Add(door);
            }

        }
        return doors;
    }

    private List<Point> GenerateCorridors()
    {
        var startingPoints = new List<Point>();
        var visitedPoints = new HashSet<Point>();
        while (true)
        {
            startingPoints.Clear();
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    if (GetDungeonTile(x, y) == Tile.None && CountNeighbors(x, y) == 0)
                        startingPoints.Add(new(x, y));
                }

            if (startingPoints.Count == 0)
                return new List<Point>(visitedPoints);

            var randomFloor = (Tile)MooseGame.Instance.Random.Next((int)Tile.FLOOR_START, (int)Tile.FLOOR_END);

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

        return GetDungeonTile(x, y) != Tile.None ? 1 : 0;
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
                x = MooseGame.Instance.Random.Next(Width - roomW);
                y = MooseGame.Instance.Random.Next(Height - roomH);
                roomRect = new(x, y, roomW, roomH);
                numTries++;
                if (numTries > roomSizes.Length * 4)
                    break;
            } while (rooms.Any(r => r.Intersects(roomRect)));

            var randomWall = (Tile)MooseGame.Instance.Random.Next((int)Tile.WALL_START, (int)Tile.WALL_END);
            DrawRoom(randomWall, randomWall, x, y, roomW, roomH);
            rooms.Add(new(x, y, roomW, roomH));
        }
        return rooms;
    }

    private void DrawRoom(Tile wallTile, Tile floorTile, int x, int y, int width, int height)
    {
        for (var i = 0; i <= width; i++)
            for (var j = 0; j <= height; j++)
                SetDungeonTile(i + x, j + y, 
                    i == 0 || j == 0 || i == width || j == height ? wallTile : floorTile);
    }

    public void FillDungeon(Tile tile)
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                SetDungeonTile(x, y, tile);

        Rooms.Clear();
        Doors.Clear();
        Coridors.Clear();
    }

    protected override int IsBlockedAt(int layer, int x, int y) 
        => DungeonLayer.GetTileValue(x, y) > Tile.BLOCKING_START ? 1 : 0;

    public void SetDungeonTile(int x, int y, Tile t)
        => DungeonLayer.SetTileValue(x, y, t);

    public Tile GetDungeonTile(int x, int y)
        => DungeonLayer.GetTileValue(x, y);
}
