using Karcero.Engine.Contracts;
using Karcero.Engine.Models;
using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Map;
public record DungeonCell : ICell
{
    public TerrainType Terrain { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public DungeonTile Tile { get; set; } = DungeonTile.None;
    public MonsterTile Monster { get; set; } = MonsterTile.None;
}
