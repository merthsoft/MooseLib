using Karcero.Engine;
using Karcero.Engine.Models;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.BaseDriver;

namespace Merthsoft.Moose.Dungeon.Map;
public class DungeonMap : BaseMap
{
    public readonly DungeonLayer DungeonLayer;
    public readonly MonsterLayer MonsterLayer;

    public override int Height => DungeonLayer.Height;
    public override int Width => DungeonLayer.Width;
    public override int TileWidth => 16;
    public override int TileHeight => 16;

    public Map<DungeonCell>? GeneratedMap { get; private set; }

    public DungeonMap(int width, int height)
    {
        AddLayer(DungeonLayer = new DungeonLayer(width, height));
        AddLayer(new ObjectLayer("player"));
        AddLayer(MonsterLayer = new MonsterLayer());
        AddLayer(new ObjectLayer("items"));
        AddLayer(new ObjectLayer("spells"));
    }

    public void ClearDungeon()
    {
        foreach (var obj in Layers.Skip(2).OfType<ObjectLayer>().SelectMany(o => o.Objects))
            obj.Remove = true;
        
        DrawRoom(DungeonTile.StoneWall, DungeonTile.None, 0, 0, Width - 1, Height - 1);
    }

    public void GenerateRandomLevel()
    {
        var dungeonGame = DungeonGame.Instance;
        ClearDungeon();
        var generator = new DungeonGenerator<DungeonCell>();
        generator.AddMapProcessor(new DungeonMapProcessor());
        var map = generator.GenerateA()
                 .DungeonOfSize(dungeonGame.DungeonSize, dungeonGame.DungeonSize)
                 .SomewhatSparse()
                 .ABitSparse()
                 .WithBigChanceToRemoveDeadEnds()
                 .WithLargeNumberOfRooms()
                 .WithLargeSizeRooms()
                 .Now();
        dungeonGame.Player.ResetVision();
        GeneratedMap = null;
        for (var i = 1; i < Width - 1; i++)
            for (var j = 1; j < Height - 1; j++)
            {
                var t = map.GetCell(i - 1, j - 1);
                if (t == null)
                    continue;
                SetDungeonTile(i, j, t.Tile);
                if (t.Monster != MonsterTile.None)
                    dungeonGame.SpawnMonster(t.Monster, i, j);
            }
        var orderedRooms = map.Rooms.OrderBy(m => m.Column).ThenBy(m => m.Row).ToList();
        var firstRoom = orderedRooms.First();
        
        var x = firstRoom.Row + 1;
        var y = firstRoom.Column + 1;
        dungeonGame.Player.Position = new Vector2(x * 16, y * 16);
        SetDungeonTile(x + 1, y + 1, DungeonTile.StairsUp);

        var ranomRoom = orderedRooms.TakeLast(4).ToList().RandomElement();
        x = ranomRoom.Row + 1;
        y = ranomRoom.Column + 1;
        SetDungeonTile(x + 1, y + 1, DungeonTile.StairsDown);
    }

    public List<Rectangle> GenerateTown(int numRooms, (int, int)[] roomSizes)
    {
        DungeonGame.Instance.Player.ResetVision();
        GeneratedMap = null;
        ClearDungeon();
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

    protected override int IsBlockedAt(string layer, int x, int y)
    {
        if (layer != "dungeon")
            return 0;

        var tile = DungeonLayer.GetTileValue(x, y);
        return tile.IsBlocking() ? (int)tile : 0;
    }

    public void SetDungeonTile(int x, int y, DungeonTile t)
        => DungeonLayer.SetTileValue(x, y, t);

    public DungeonTile GetDungeonTile(int x, int y)
        => DungeonLayer.GetTileValue(x, y);
}
