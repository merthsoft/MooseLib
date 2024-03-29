﻿using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Tiles;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Renderers;
public class DungeonRenderer : SpriteBatchFourWayAutoTileTextureRenderer
{
    static Color FogOfWarColor = Color.Black.HalveAlphaChannel();

    public DungeonRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
    }

    protected override int GetNeighborValue(DungeonTile _tile, int x, int y, ITileLayer<DungeonTile> layer, int neighborValue)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;

        if (layer.GetTileValue(x, y) < DungeonTile.WALL_START || DungeonPlayer.Instance.CanSee(x, y) == FogOfWar.Full)
            return 0;

        return neighborValue;
    }

    public override void DrawSprite(int spriteIndex, DungeonTile tile, int i, int j, ITileLayer<DungeonTile> layer,  Vector2 drawOffset, float layerDepth = 1)
    {
        var rect = GetDestinationRectangle(i, j, drawOffset);
        if (rect == null)
            return;

        if (DungeonPlayer.Instance.CanSee(i, j) != FogOfWar.Full)
            base.DrawSprite(spriteIndex, tile, i, j, layer, drawOffset, 0.5f);
        if (DungeonPlayer.Instance.CanSee(i, j) == FogOfWar.Half)
            SpriteBatch.FillRectangle(rect.Value, FogOfWarColor, 1);
    }
}
