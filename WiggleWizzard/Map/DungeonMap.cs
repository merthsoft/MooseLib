using Karcero.Engine;
using Karcero.Engine.Models;
using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Items;
using Merthsoft.Moose.Dungeon.Entities.Monsters;
using Merthsoft.Moose.Dungeon.Entities.Spells;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.PathFinding.Maps;
using Merthsoft.Moose.MooseEngine.PathFinding.Paths.AStar;

namespace Merthsoft.Moose.Dungeon.Map;
public class DungeonMap : PathFinderMap
{
    public readonly DungeonLayer DungeonLayer;
    public readonly ObjectLayer<DungeonMonster> MonsterLayer;
    public IEnumerable<DungeonMonster> Monsters => MonsterLayer.Objects;

    public readonly ObjectLayer<DungeonItem> ItemLayer;
    public IEnumerable<DungeonItem> Items => ItemLayer.Objects;

    public readonly ObjectLayer<Spell> SpellLayer;
    public IEnumerable<Spell> Spells => SpellLayer.Objects;

    public override int Height => DungeonLayer.Height;
    public override int Width => DungeonLayer.Width;
    public override int TileWidth => 16;
    public override int TileHeight => 16;

    public int SeedUsed;
    public Map<DungeonCell>? GeneratedMap;

    public List<Rectangle> Rooms = new();

    public WeightedSet<ItemTile> Treasures = new();

    public DungeonMap(int width, int height) : base(new AStarPathFinder())
    {
        DungeonLayer = AddLayer(new DungeonLayer(width, height));
        AddLayer(new ObjectLayer<DungeonPlayer>("player", width, height));
        ItemLayer = AddLayer(new ObjectLayer<DungeonItem>("items", width, height));
        MonsterLayer = AddLayer(new ObjectLayer<DungeonMonster>("monsters", width, height));
        SpellLayer = AddLayer(new ObjectLayer<Spell>("spells", width, height));
    }

    public void ClearDungeon()
    {
        Rooms.Clear();
        foreach (var obj in Layers.Skip(2).OfType<IObjectLayer>().SelectMany(o => o.Objects))
            obj.Remove = true;

        DrawRoom(DungeonTile.StoneWall, DungeonTile.None, 0, 0, Width - 1, Height - 1);
        SeedUsed = -1;
    }

    DungeonTile RandomFloor()
        => (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.FLOOR_START, (int)DungeonTile.FLOOR_END);

    DungeonTile RandomDoor()
        => (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.DOOR_START, (int)DungeonTile.DOOR_END);

    DungeonTile NeighborFloor(Map<DungeonCell> map, int x, int y)
    {
        var spot = map.GetCell(x - 1, y)?.Tile;
        if (spot?.IsFloor() ?? false)
            return spot.Value;
        spot = map.GetCell(x + 1, y)?.Tile;
        if (spot?.IsFloor() ?? false)
            return spot.Value;
        spot = map.GetCell(x, y - 1)?.Tile;
        if (spot?.IsFloor() ?? false)
            return spot.Value;
        spot = map.GetCell(x, y + 1)?.Tile;
        if (spot?.IsFloor() ?? false)
            return spot.Value;

        return RandomFloor();
    }

    bool NeighborEmpty(int x, int y)
    {
        var spot = DungeonLayer.GetTileValue(x - 1, y);
        if (spot == DungeonTile.None)
            return true;
        spot = DungeonLayer.GetTileValue(x + 1, y);
        if (spot == DungeonTile.None)
            return true;
        spot = DungeonLayer.GetTileValue(x, y - 1);
        if (spot == DungeonTile.None)
            return true;
        spot = DungeonLayer.GetTileValue(x, y + 1);
        if (spot == DungeonTile.None)
            return true;

        return false;
    }

    public void GenerateDungeon(int? seed = null)
    {
        Treasures.Clear();

        foreach (var t in Enum.GetValues<ItemTile>())
        {
            var val = t.TreasureValue();
            if (val == -1)
                continue;
            Treasures.Add(t, (20 - val) * 5);
        }

        var roomTypes = new DungeonRoomDef[]
        {
            new(.5f) { NumTreasure = 1 },
            new(.5f) { NumMonsters = 1, MonsterLevelDelta = 0 },
            new(.4f) { NumMonsters = 2, MonsterLevelDelta = 0 },
            new(.05f) { NumMonsters = 3, NumChests = 1 },
            //new(.1f) { NumMonsters = 1, MonsterLevelDelta = 1 },
            new(.15f) { NumChests = 1 },
            //new(.15f) { NumPotions = 1 },
            //new(.15f) { NumScrolls = 1 },
        };
        var weightedRooms = new WeightedSet<DungeonRoomDef>();
        for (var roomNumber = 0; roomNumber < roomTypes.Length; roomNumber++)
        {
            var room = roomTypes[roomNumber];
            room.RoomNumber = roomNumber;
            weightedRooms.Add(room, (int)(100 * room.Weight));
        }

        var dungeonGame = WiggleWizzardGame.Instance;
        ClearDungeon();
        var generator = new DungeonGenerator<DungeonCell>();
        if (seed != null)
            SeedUsed = seed.Value;
        else
            SeedUsed = Math.Abs((int)DateTime.UtcNow.Ticks);
        var map = generator.GenerateA()
                 .DungeonOfSize(dungeonGame.DungeonSize - 2, dungeonGame.DungeonSize - 2)
                 .ABitRandom()
                 .ABitSparse()
                 .WithBigChanceToRemoveDeadEnds()
                 .WithLargeNumberOfRooms()
                 .WithRoomSize(4, 8, 4, 8)
                 .WithSeed(SeedUsed)
                 .Now();
        dungeonGame.Player.ResetVision();
        GeneratedMap = map;

        var treasureEnumerator = Treasures.GetEnumerator();
        var roomTypeEnumerator = weightedRooms.GetEnumerator();

        for (var i = 0; i < map.Rooms.Count; i++)
        {
            var room = map.Rooms[i];
            Rooms.Add(new(room.Top, room.Left, room.Size.Height + 1, room.Size.Width + 1));
            OverlayWalls(DungeonTile.BrickWall, room.Top, room.Left, room.Size.Height + 1, room.Size.Width + 1);

            if (i != 0 && i != map.Rooms.Count)
            {
                var middle = new Point(room.Top + room.Size.Height / 2, room.Left + room.Size.Width / 2);
                var spiral = middle.SpiralAround().GetEnumerator();

                (int x, int y) moveSpiral()
                {
                    int spotX;
                    int spotY;
                    do
                    {
                        spiral.MoveNext();
                        (spotX, spotY) = spiral.Current;
                    } while (IsCellOccupied(spotX, spotY));
                    return (spotX, spotY);
                }

                while (!roomTypeEnumerator.MoveNext())
                    roomTypeEnumerator = weightedRooms.GetEnumerator();

                var roomType = roomTypeEnumerator.Current;

                for (var treasureCount = 0; treasureCount < roomType.NumTreasure; treasureCount++)
                {
                    while (!treasureEnumerator.MoveNext())
                        treasureEnumerator = Treasures.GetEnumerator();
                    var (spotX, spotY) = moveSpiral();
                    var randomTreasure = treasureEnumerator.Current;
                    dungeonGame.SpawnItem(randomTreasure, spotX, spotY);
                }

                for (var chestCount = 0; chestCount < roomType.NumChests; chestCount++)
                {
                    var (spotX, spotY) = moveSpiral();
                    var roomChest = (Chest)dungeonGame.SpawnItem(ItemTile.ClosedChest, spotX, spotY);
                    roomChest.Contents.AddRange(Treasures.Take(dungeonGame.Random.Next(3, 7)));
                }

                for (var monsterCount = 0; monsterCount < roomType.NumMonsters; monsterCount++)
                {
                    var (spotX, spotY) = moveSpiral();
                    if (dungeonGame.Random.NextSingle() < .5f)
                        if (dungeonGame.Random.NextSingle() < .5f)
                            dungeonGame.SpawnMonster(MonsterTile.BlueSlime, spotX, spotY);
                        else
                            dungeonGame.SpawnMonster(MonsterTile.GreenSlime, spotX, spotY);
                    else
                        if (dungeonGame.Random.NextSingle() < .5f)
                            dungeonGame.SpawnMonster(MonsterTile.PinkSnake, spotX, spotY);
                        else
                            dungeonGame.SpawnMonster(MonsterTile.GreenSnake, spotX, spotY);
                }
            }
        }

        for (var i = 1; i < Width - 1; i++)
            for (var j = 1; j < Height - 1; j++)
            {
                var t = map.GetCell(i - 1, j - 1);
                if (t == null)
                    continue;

                var mapTile = DungeonLayer.GetTileValue(i, j);
                var tile = t.Terrain switch
                {
                    TerrainType.Door => DungeonTile.DOOR_START,
                    TerrainType.Floor => NeighborFloor(map, i - 1, j - 1),
                    _ => mapTile.IsWall() ? DungeonLayer.GetTileValue(i, j) : DungeonTile.StoneWall,
                };

                t.Tile = tile;
                SetDungeonTile(i, j, tile);
            }
        var firstRoom = Rooms.First();

        var x = firstRoom.X + 1;
        var y = firstRoom.Y + 1;
        var width = firstRoom.Width - 2;
        var height = firstRoom.Height - 2;
        dungeonGame.Player.Position = new Vector2(x * 16, y * 16);
        SetDungeonTile(x + 1, y + 1, DungeonTile.StairsUp);

        var chest = (dungeonGame.SpawnItem(ItemTile.ClosedChest, x + width / 2, y + height / 2) as Chest)!;
        chest.Contents.Add(ItemTile.SCROLL_START);
        chest.Contents.Add(ItemTile.POTION_START);
        chest.Contents.AddRange(Treasures.Take(dungeonGame.Random.Next(3, 5)));

        var lastRoom = Rooms.Last();
        x = lastRoom.X + 1;
        y = lastRoom.Y + 1;
        width = lastRoom.Width - 2;
        height = lastRoom.Height - 2;
        SetDungeonTile(x + 1, y + 1, DungeonTile.StairsDown);
        dungeonGame.SpawnMonster(MonsterTile.Marshall, x, y);

        chest = (dungeonGame.SpawnItem(ItemTile.ClosedChest, x + width / 2, y + height / 2) as Chest)!;
        chest.Contents.AddRange(Treasures.Take(dungeonGame.Random.Next(10, 20)));
    }

    public void GenerateTown(int numRooms, (int, int)[] roomSizes, int? seed = null)
    {
        ClearDungeon();
        if (seed != null)
            SeedUsed = seed.Value;
        else
            SeedUsed = Math.Abs((int)DateTime.UtcNow.Ticks);

        var gamme = WiggleWizzardGame.Instance;
        gamme.SetSeed(SeedUsed);

        gamme.Player.ResetVision();
        GeneratedMap = null;
        var allRooms = GenerateRooms(numRooms + 1, roomSizes);
        var stairRoom = allRooms.RemoveRandomElement();
        PlaceStairs(stairRoom);
        Rooms.AddRange(allRooms);
        GenerateCorridors(Rooms);
        HollowOutRooms(Rooms);
        GenerateDoors(Rooms);
    }

    private void PlaceStairs(Rectangle rect)
    {
        var (x, y, width, height) = rect;
        DrawRoom(RandomFloor(), RandomFloor(), x, y, width, height);

        var tileX = x + width / 2;
        var tileY = y + height / 2;
        DungeonLayer.SetTileValue(tileX, tileY, DungeonTile.StairsDown);
        DungeonPlayer.Instance.Position = new Vector2((tileX + 1)*16, tileY*16);
    }

    public bool IsCellOccupied(int x, int y)
    {
        var dungeonTile = DungeonLayer.GetTileValue(x, y);
        if (dungeonTile != DungeonTile.None && !dungeonTile.IsFloor())
            return true;
        
        if (MonsterLayer.GetObjects(x, y).Any()
            || ItemLayer.GetObjects(x, y).Any()
            || SpellLayer.GetObjects(x, y).Any())
            return true;

        return false;
    }

    private void HollowOutRooms(List<Rectangle> rooms)
    {
        foreach (var (x, y, width, height) in rooms)
            DrawRoom(GetDungeonTile(x, y), RandomFloor(), x, y, width, height);
    }

    private List<Point> GenerateDoors(List<Rectangle> rooms)
    {
        var doors = new List<Point>();
        foreach (var (x, y, w, h) in rooms)
        {
            var spots = new List<Point>();
            int sX, sY;

            for (var mult = 2; mult <= 2; mult += 1)
                spots.AddRange(new[] 
                {
                    new Point(x + w/mult, y),
                    new Point(x, y + h/mult),
                    new Point(x + w/mult, y + h),
                    new Point(x + w, y + h/mult),
                });

            do
            {
                (sX, sY) = spots.RemoveRandomElement();
            } while (!NeighborEmpty(sX, sY) && spots.Any());

            if (!spots.Any())
                continue;

            DungeonLayer.SetTileValue(sX, sY, RandomDoor());
        }
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

    private void OverlayWalls(DungeonTile wallTile, int x, int y, int width, int height)
    {
        for (var i = 0; i <= width; i++)
        {
            SetDungeonTile(i + x, y, wallTile);
            SetDungeonTile(i + x, y + height, wallTile);
        }
        for (var j = 0; j <= height; j++)
        {
            SetDungeonTile(x, j + y, wallTile);
            SetDungeonTile(x + width, j + y, wallTile);
        }
    }

    public void FillDungeon(DungeonTile tile)
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                SetDungeonTile(x, y, tile);
    }

    public override int IsBlockedAt(string layer, int x, int y)
    {
        switch (layer)
        {
            case "dungeon":
                var tile = DungeonLayer.GetTileValue(x, y);
                return tile.IsBlocking() ? (int)tile : 0;
            case "monsters":
                return Monsters.FirstOrDefault(m => m.InCell(x, y))?.DrawIndex ?? 0;
            case "items":
                var item = Items.FirstOrDefault(m => m.InCell(x, y));
                if (item?.ItemDef?.BlocksPlayer ?? false)
                    return item.DrawIndex;
                return 0;
            case "spells":
                var spell = Spells.FirstOrDefault(m => m.InCell(x, y));
                if (spell?.SpellDef.BlocksPlayer ?? false)
                    return 1;
                return 0;
        }

        return 0;
    }

    public bool IsBlockedAt(int x, int y)
        => GetBlockingVector(x, y).Any(i => i > 0);

    public void SetDungeonTile(int x, int y, DungeonTile t)
        => DungeonLayer.SetTileValue(x, y, t);

    public DungeonTile GetDungeonTile(int x, int y)
        => DungeonLayer.GetTileValue(x, y);
}
