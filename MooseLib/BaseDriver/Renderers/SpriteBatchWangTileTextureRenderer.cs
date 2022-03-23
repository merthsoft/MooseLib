using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchWangTileTextureRenderer<TTile> : SpriteBatchTextureRenderer<TTile> where TTile : struct
{
    public Dictionary<TTile, int> WangDefinitions = new();
    public List<WangTile> WangTiles = new();
    public Dictionary<string, List<WangTile>> Cache = new();

    public SpriteBatchWangTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) 
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        
    }

    public virtual void RebuildCache()
    {
        Cache.Clear();
        foreach (var tile in WangTiles.GroupBy(w => w.WangId))
            Cache[tile.Key] = tile.ToList();
    }

    protected virtual WangTile GetWangId(int x, int y, ITileLayer<TTile> layer)
    {
        var baseTile = layer.GetTileValue(x, y);
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

    protected virtual int GetNeighborValue(int x, int y, TTile baseTile, ITileLayer<TTile> layer)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;
        var neighborValue = layer.GetTileValue(x, y);
        return WangDefinitions.GetValueOrDefault(neighborValue, 0);
        
    }

    public override void DrawSprite(int spriteIndex, TTile tile, int i, int j, ITileLayer<TTile> layer, Vector2 drawOffset, float layerDepth = 1)
    {
        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset + drawOffset);
        if (destRect == null)
            return;

        var wangDef = WangDefinitions.GetValueOrDefault(tile, -1);

        var wangTile = GetWangId(i, j, layer);
        var wangId = wangTile.WangId;

        var tileIndex = Cache.GetValueOrDefault(wangId);
        if (tileIndex == null || !tileIndex.Any(t => t.AppliesTo.HasValue ? t.AppliesTo.Value == wangDef : true)) {
            var tileGroup = WangTiles.Where(t => t.AppliesTo.HasValue ? t.AppliesTo.Value == wangDef : true)
                                     .GroupBy(t => t.Compare(wangTile)).OrderByDescending(g => g.Key).FirstOrDefault();

            if (wangDef == -1 || tileGroup == null || tileGroup.Key == int.MinValue)
            {
                SpriteBatch.Draw(SpriteSheet,
                    position: destRect.Value.Position,
                    sourceRectangle: GetSourceRectangle(spriteIndex),
                    color: Color, rotation: Rotation, effects: SpriteEffects,
                    origin: Vector2.Zero, scale: DrawScale, layerDepth: layerDepth);
                return;
            }
            tileIndex = tileGroup.ToList();
            if (!Cache.ContainsKey(wangId))
                Cache[wangId] = new();
            Cache[wangId].AddRange(tileIndex);
        }
        var validItems = tileIndex.Where(t => t.AppliesTo.HasValue ? t.AppliesTo.Value == wangDef : true).ToList();
        var rand = new Random(i * layer.Width + j).Next(validItems.Count);
        SpriteBatch.Draw(SpriteSheet,
                position: destRect.Value.Position, scale: DrawScale,
                sourceRectangle: GetSourceRectangle(validItems[rand].TileId, SpriteSheet),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);
    }
}
