using Karcero.Engine.Contracts;
using Karcero.Engine.Models;
using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Map;
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
        for (var y = 0; y < map.Height; y++)
            for (var x = 0; x < map.Width; x++)
            {
                var t = map.GetCell(x, y);
                t.Tile = t.Terrain switch
                {
                    TerrainType.Door => DungeonTile.DOOR_START,
                    TerrainType.Floor => NeighborFloor(map, x, y),
                    _ => DungeonTile.WALL_START,
                };
            }
        var firstRoom = map.Rooms.OrderBy(m => m.Column).ThenBy(m => m.Row).First();
        var monsterCell = map.GetCell(firstRoom.Bottom - 1, firstRoom.Right - 1);
        monsterCell.Monster = MonsterTile.Marshall;
    }
}   