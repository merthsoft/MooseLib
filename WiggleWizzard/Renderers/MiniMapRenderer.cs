using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Renderers;
public class MiniMapRenderer : SpriteBatchFourWayAutoTileTextureRenderer<MiniMapTile>
{
    static Color FogOfWarColor = Color.Black.HalveAlphaChannel();

    public MiniMapRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, 
        int textureMargin = 0, int tilePadding = 0) 
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        DrawOffset = new(10, 10);
    }

    protected override int GetNeighborValue(MiniMapTile _tile, int x, int y, ITileLayer<MiniMapTile> layer, int neighborValue)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;

        if (layer.GetTileValue(x, y) < MiniMapTile.WALL_START)
            return 0;

        return neighborValue;
    }

    public override void DrawSprite(int spriteIndex, MiniMapTile tile, int i, int j, ITileLayer<MiniMapTile> layer, Vector2 drawOffset, float layerDepth = 1)
    {
        var rect = GetDestinationRectangle(i, j, drawOffset);
        if (rect == null)
            return;

        if (DungeonPlayer.Instance.GetMiniMapTile(i, j) != MiniMapTile.None)
            base.DrawSprite(spriteIndex, tile, i, j, layer, drawOffset, layerDepth);
        if (DungeonPlayer.Instance.CanSee(i, j) == FogOfWar.Half)
            SpriteBatch.FillRectangle(rect.Value, FogOfWarColor, 1f);
    }
}
