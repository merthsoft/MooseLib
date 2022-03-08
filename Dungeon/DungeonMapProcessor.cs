﻿using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Merthsoft.Moose.Dungeon;
public class DungeonMapProcessor : IMapProcessor<DungeonCell>
{
    DungeonTile RandomFloor() 
        => (DungeonTile)MooseGame.Instance.Random.Next((int)DungeonTile.FLOOR_START, (int)DungeonTile.FLOOR_END);
    
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

                if (DungeonGame.Player.Position == Vector2.Zero && t.Terrain == TerrainType.Floor)
                    DungeonGame.Player.Position = new(t.Row * 16 + 16, t.Column * 16 + 16);
            }
    }
}