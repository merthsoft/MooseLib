using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Merthsoft.Moose.Dungeon;
public record DungeonCell : ICell
{
    public TerrainType Terrain { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public DungeonTile Tile { get; set; } = DungeonTile.None;
}
