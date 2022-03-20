using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchWangTileTextureRenderer<TTile> : SpriteBatchTextureRenderer<TTile> where TTile : struct
{
    public Dictionary<TTile, int> WangDefinitions = new();
    public List<WangTile> WangTiles = new();

    public SpriteBatchWangTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) 
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        
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

        var neighborTile = layer.GetTileValue(x, y);
        if (EqualityComparer<TTile>.Default.Equals(neighborTile, baseTile))
            return WangDefinitions.GetValueOrDefault(baseTile);
        else
            return 0;
    }

    public override void DrawSprite(int spriteIndex, TTile tile, int i, int j, ITileLayer<TTile> layer, Vector2 drawOffset, float layerDepth = 1)
    {
        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset + drawOffset);
        if (destRect == null)
            return;

        var wangDef = WangDefinitions.GetValueOrDefault(tile, -1);

        var wangId = GetWangId(i, j, layer);
        var tileIndex = WangTiles.OrderByDescending(t => t.Compare(wangId)).FirstOrDefault();

        if (wangDef == -1 || tileIndex == null || tileIndex.Compare(wangId) == int.MinValue)
        {
                SpriteBatch.Draw(SpriteSheet,
                    position: destRect.Value.Position,
                    sourceRectangle: GetSourceRectangle(spriteIndex),
                    color: Color, rotation: Rotation, effects: SpriteEffects,
                    origin: Vector2.Zero, scale: DrawScale, layerDepth: layerDepth);
            return;
        }

        SpriteBatch.Draw(SpriteSheet,
                position: destRect.Value.Position, scale: DrawScale,
                sourceRectangle: GetSourceRectangle(tileIndex.TileId, SpriteSheet),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);
    }
}
