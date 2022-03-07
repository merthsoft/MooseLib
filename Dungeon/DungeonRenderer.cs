using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;
internal class DungeonRenderer : SpriteBatchAutoTileTextureRenderer
{
    public DungeonRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
    }

    protected override int GetNeighborValue(int _tile, int x, int y, ITileLayer<int> layer, int neighborValue)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;

        if (layer.GetTileValue(x, y) < (int)Tile.WALL_START)
            return 0;

        return neighborValue;
    }
}
