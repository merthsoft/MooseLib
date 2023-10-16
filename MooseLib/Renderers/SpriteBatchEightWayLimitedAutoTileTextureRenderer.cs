using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.Renderers;

public class SpriteBatchEightWayLimitedAutoTileTextureRenderer : SpriteBatchFourWayAutoTileTextureRenderer
{
    // This renderer assumes everyting is in 2x2 chunks, so certain arrangements aren't possible
    private Dictionary<int, int> indexMap = new()
    {
        [0b00000000] = 0,
        [0b10000000] = 1,
        [0b11100000] = 2,
        [0b00100000] = 3,
        [0b10010100] = 4,
        [0b00101001] = 5,
        [0b00000100] = 6,
        [0b00000111] = 7,
        [0b00000001] = 8,
        [0b00101111] = 9,
        [0b10010111] = 10,
        [0b11101001] = 11,
        [0b11110100] = 12,
        [0b00000011] = 7,
        [0b00001001] = 5,
        [0b01100000] = 2,
        [0b00000110] = 7,
        [0b00010100] = 4,
        [0b11010100] = 2,
        [0b10010000] = 4,
        [0b00101000] = 5,
        [0b11000000] = 2
    };

    public SpriteBatchEightWayLimitedAutoTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0)
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {

    }

    protected override int GetTileIndex(int tile, int neighborCount)
        => indexMap.GetValueOrDefault(neighborCount);

    protected override int CountNeighbors(int tile, int x, int y, ITileLayer layer)
    {
        var total = 0;
        var north = GetNeighborValue(tile, x + 0, y + -1, layer, 0b00000010);
        total += north;
        var west = GetNeighborValue(tile, x + -1, y + 0, layer, 0b00001000);
        total += west;
        var east = GetNeighborValue(tile, x + 1, y + 0, layer, 0b00010000);
        total += east;
        var south = GetNeighborValue(tile, x + 0, y + 1, layer, 0b01000000);
        total += south;

        total += GetNeighborValue(tile, x + -1, y + -1, layer, 0b00000001);
        total += GetNeighborValue(tile, x + 1, y + -1, layer, 0b00000100);
        total += GetNeighborValue(tile, x + -1, y + 1, layer, 0b00100000);
        total += GetNeighborValue(tile, x + 1, y + 1, layer, 0b10000000);

        return total;
    }
}
