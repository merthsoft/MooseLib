using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon;
public class DungeonRenderer : SpriteBatchAutoTileTextureRenderer
{
    private readonly DungeonPlayer player;

    public DungeonRenderer(DungeonPlayer player, SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        this.player = player;
    }

    protected override int GetNeighborValue(int _tile, int x, int y, ITileLayer<int> layer, int neighborValue)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;

        if (layer.GetTileValue(x, y) < (int)DungeonTile.WALL_START)
            return 0;

        return neighborValue;
    }

    public override void DrawSprite(int spriteIndex, int i, int j, int layerNumber, ITileLayer<int> layer, float layerDepth = 1)
    {
        if (player.CanSee(i, j))
            base.DrawSprite(spriteIndex, i, j, layerNumber, layer, layerDepth);
    }
}
