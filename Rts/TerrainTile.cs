using System;
using System.Linq;

namespace Merthsoft.Moose.Rts;
internal enum TerrainTile
{
    Water = 0,
    WaterEdge_Start,
    WaterEdge_End = WaterEdge_Start + 11,
    WaterDecoration_Start,
    WaterDecoration_End,
    Land,
    Grass_Start,
    Grass_End = Grass_Start + 6,
    Flower_Start,
    Flower_End = Flower_Start + 11
}
