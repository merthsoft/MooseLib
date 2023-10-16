using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.Renderers;

public class SpriteBatchEightWayAutoTileTextureRenderer : SpriteBatchFourWayAutoTileTextureRenderer
{
    //https://code.tutsplus.com/how-to-use-tile-bitmasking-to-auto-tile-your-level-layouts--cms-25673t
    private Dictionary<int, int> indexMap = new()
    {
        { 2 , 1 },
        { 8 , 2 },
         { 10 , 3 },
         { 11 , 4 },
         { 16 , 5 },
         { 18 , 6 },
         { 22 , 7 },
         { 24 , 8 },
         { 26 , 9 },
         { 27 , 10 },
         { 30 , 11 },
         { 31 , 12 },
         { 64 , 13 },
         { 66 , 14 },
         { 72 , 15 },
         { 74 , 16 },
         { 75 , 17 },
         { 80 , 18 },
         { 82 , 19 },
         { 86 , 20 },
         { 88 , 21 },
         { 90 , 22 },
         { 91 , 23 },
         { 94 , 24 },
         { 95 , 25 },
         { 104 , 26 },
         { 106 , 27 },
         { 107 , 28 },
         { 120 , 29 },
         { 122 , 30 },
         { 123 , 31 },
         { 126 , 32 },
         { 127 , 33 },
         { 208 , 34 },
         { 210 , 35 },
         { 214 , 36 },
         { 216 , 37 },
         { 218 , 38 },
         { 219 , 39 },
         { 222 , 40 },
         { 223 , 41 },
         { 248 , 42 },
         { 250 , 43 },
         { 251 , 44 },
         { 254 , 45 },
         { 255 , 46 },
         { 0 , 47 }
    };

    public SpriteBatchEightWayAutoTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0)
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {

    }

    protected override int CountNeighbors(int tile, int x, int y, ITileLayer layer)
    {
        var total = 0;
        var north = GetNeighborValue(tile, x + 0, y + -1, layer, 2);
        total += north;
        var west = GetNeighborValue(tile, x + -1, y + 0, layer, 8);
        total += west;
        var east = GetNeighborValue(tile, x + 1, y + 0, layer, 16);
        total += east;
        var south = GetNeighborValue(tile, x + 0, y + 1, layer, 64);
        total += south;

        if (north > 0 && west > 0)
            total += GetNeighborValue(tile, x + -1, y + -1, layer, 1);

        if (north > 0 && east > 0)
            total += GetNeighborValue(tile, x + 1, y + -1, layer, 4);

        if (south > 0 && west > 0)
            total += GetNeighborValue(tile, x + -1, y + 1, layer, 32);

        if (south > 0 && east > 0)
            total += GetNeighborValue(tile, x + 1, y + 1, layer, 128);

        return total;
    }

    protected override int GetTileIndex(int tile, int neighborCount)
        => indexMap[tile];
}
