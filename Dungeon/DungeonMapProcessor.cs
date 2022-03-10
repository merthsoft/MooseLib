using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Merthsoft.Moose.Dungeon;
public class DungeonMapProcessor : IMapProcessor<DungeonCell>
{
    DungeonTile RandomFloor()
        => (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.FLOOR_START, (int)DungeonTile.FLOOR_END);

    DungeonTile RandomWall()
        => (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.WALL_START, (int)DungeonTile.WALL_END);

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

    public void ProcessMap(Map<DungeonCell> map, DungeonConfiguration configuration, IRandomizer randomizer)
    {

        foreach (var room in map.Rooms)
        {
            var roomX = room.Row;
            var roomY = room.Column;
            var roomWidth = room.Size.Width;
            var roomHeight = room.Size.Height;
            var wallTile = RandomWall();
            var floorTile = RandomFloor();

            for (var x = 0; x < roomWidth; x++)
                for (var y = 0; y < roomHeight; y++)
                {
                    var t = map.GetCell(i + x, j + y);
                    t.Tile = i == 0 || j == 0 || i == roomWidth || j == roomHeight ? wallTile : floorTile;
                }
        }

        for (var y = 0; y < map.Height; y++)
            for (var x = 0; x < map.Width; x++)
            {
                var t = map.GetCell(x, y);
                t.Tile = t.Terrain switch
                {
                    TerrainType.Door => DungeonTile.DOOR_START,
                    TerrainType.Floor => NeighborFloor(map, x, y),
                    _ => DungeonTile.None,
                };
            }
        var firstRoom = map.Rooms.OrderBy(m => m.Column).ThenBy(m => m.Row).First();
        var monsterCell = map.GetCell(firstRoom.Bottom - 1, firstRoom.Right - 1);
        monsterCell.Monster = MonsterTile.Marshall;
    }
}   