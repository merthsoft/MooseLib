using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Renderers;
public class MiniMapRenderer : SpriteBatchAutoTileTextureRenderer
{
    public MiniMapRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, 
        int textureMargin = 0, int tilePadding = 0) 
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        DrawOffset = new(10, 10);
    }

    protected override int GetNeighborValue(int _tile, int x, int y, ITileLayer<int> layer, int neighborValue)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;

        if (layer.GetTileValue(x, y) < (int)MiniMapTile.WALL_START)
            return 0;

        return neighborValue;
    }

    public override void DrawSprite(int spriteIndex, int i, int j, ITileLayer<int> layer, Vector2 drawOffset, float layerDepth = 1)
    {
        if (DungeonPlayer.Instance.GetMiniMapTile(i, j) != MiniMapTile.None)
            base.DrawSprite(spriteIndex, i, j, layer, drawOffset, layerDepth);
    }
}
