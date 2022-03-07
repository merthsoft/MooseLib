using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Merthsoft.Moose.Dungeon;
internal record DungeonCell : ICell
{
    public TerrainType Terrain { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public Tile Tile { get; set; } = Tile.None;
}
