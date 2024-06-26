﻿using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;

public class SpriteBatchWangTileTextureRenderer : SpriteBatchTileTextureRenderer
{
    public Dictionary<int, int> WangDefinitions = [];
    public List<WangTile> WangTiles = [];
    public Dictionary<string, List<WangTile>> Cache = [];

    public SpriteBatchWangTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0)
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {

    }

    public virtual void RebuildCache()
    {
        Cache.Clear();
        foreach (var tile in WangTiles.GroupBy(w => w.WangId))
            Cache[tile.Key] = [.. tile];
    }

    protected virtual WangTile GetWangId(int x, int y, ITileLayer layer)
    {
        var baseTile = layer.GetTileIndex(x, y);
        var wangTile = new WangTile(-1);
        wangTile.North.Add(GetNeighborValue(x, y - 1, baseTile, layer));
        wangTile.NorthEast.Add(GetNeighborValue(x + 1, y - 1, baseTile, layer));
        wangTile.East.Add(GetNeighborValue(x + 1, y, baseTile, layer));
        wangTile.SouthEast.Add(GetNeighborValue(x + 1, y + 1, baseTile, layer));
        wangTile.South.Add(GetNeighborValue(x, y + 1, baseTile, layer));
        wangTile.SouthWest.Add(GetNeighborValue(x - 1, y + 1, baseTile, layer));
        wangTile.West.Add(GetNeighborValue(x - 1, y, baseTile, layer));
        wangTile.NorthWest.Add(GetNeighborValue(x - 1, y - 1, baseTile, layer));
        return wangTile;
    }

    protected virtual int GetNeighborValue(int x, int y, int baseTile, ITileLayer layer)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;
        var neighborValue = layer.GetTileIndex(x, y);
        return WangDefinitions.GetValueOrDefault(neighborValue, 0);

    }

    public override void DrawSprite(int spriteIndex, int i, int j, ILayer layer, Vector2 drawOffset, float layerDepth = 1)
    {
        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset + drawOffset);
        if (destRect == null)
            return;

        var wangDef = WangDefinitions.GetValueOrDefault(spriteIndex, -1);

        var wangTile = GetWangId(i, j, layer as ITileLayer);
        var wangId = wangTile.WangId;

        var tileIndex = Cache.GetValueOrDefault(wangId);
        if (tileIndex == null || !tileIndex.Any(t => t.AppliesTo.HasValue ? t.AppliesTo.Value == wangDef : true))
        {
            var tileGroup = WangTiles.Where(t => t.AppliesTo.HasValue ? t.AppliesTo.Value == wangDef : true)
                                     .GroupBy(t => t.Compare(wangTile)).OrderByDescending(g => g.Key).FirstOrDefault();

            if (wangDef == -1 || tileGroup == null || tileGroup.Key == int.MinValue)
            {
                SpriteBatch.Draw(SpriteSheet,
                    position: destRect.Value.Position,
                    sourceRectangle: GetSourceRectangle(spriteIndex),
                    color: layer.DrawColor, rotation: Rotation, effects: SpriteEffects,
                    origin: Vector2.Zero, scale: DrawScale, layerDepth: layerDepth);
                return;
            }
            tileIndex = [.. tileGroup];
            if (!Cache.ContainsKey(wangId))
                Cache[wangId] = [];
            Cache[wangId].AddRange(tileIndex);
        }
        var validItems = tileIndex.Where(t => t.AppliesTo.HasValue ? t.AppliesTo.Value == wangDef : true).ToList();
        var rand = new Random(i * layer.Width + j).Next(validItems.Count);
        SpriteBatch.Draw(SpriteSheet,
                position: destRect.Value.Position, scale: DrawScale,
                sourceRectangle: GetSourceRectangle(validItems[rand].TileId, SpriteSheet),
                color: layer.DrawColor, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);
    }
}
