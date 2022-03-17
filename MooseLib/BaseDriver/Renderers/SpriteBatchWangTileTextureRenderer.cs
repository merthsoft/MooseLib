using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchWangTileTextureRenderer<TTile> : SpriteBatchTextureRenderer<TTile> where TTile : struct
{
    public Dictionary<TTile, Texture2D> AutoTileTextureMap = new();

    public Texture2D this[TTile index]
    {
        get => AutoTileTextureMap[index];
        set => AutoTileTextureMap[index] = value;
    }

    public Dictionary<TTile, string> WangTiles = new();


    public SpriteBatchWangTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) 
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        
    }

    protected virtual int GetTileIndex(TTile tile, int neighborCount)
        => neighborCount;

    protected virtual int CountNeighbors(TTile tile, int x, int y, ITileLayer<TTile> layer)
        => GetNeighborValue(tile, x +  0, y + -1, layer, 1) 
         + GetNeighborValue(tile, x + -1, y +  0, layer, 2) 
         + GetNeighborValue(tile, x +  1, y +  0, layer, 4) 
         + GetNeighborValue(tile, x +  0, y +  1, layer, 8);

    protected virtual int GetNeighborValue(TTile tile, int x, int y, ITileLayer<TTile> layer, int neighborValue)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;

        if (EqualityComparer<TTile>.Default.Equals(tile, layer.GetTileValue(x, y)))
            return 0;

        return neighborValue;
    }

    public override void DrawSprite(int spriteIndex, TTile tile, int i, int j, ITileLayer<TTile> layer, Vector2 drawOffset, float layerDepth = 1)
    {
        var texture = AutoTileTextureMap.GetValueOrDefault(tile);

        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset + drawOffset);
        if (destRect == null)
            return;

        if (texture == null)
        {
                SpriteBatch.Draw(SpriteSheet,
                    position: destRect.Value.Position,
                    sourceRectangle: GetSourceRectangle(spriteIndex),
                    color: Color, rotation: Rotation, effects: SpriteEffects,
                    origin: Vector2.Zero, scale: DrawScale, layerDepth: layerDepth);
            return;
        }

        var neighborCount = CountNeighbors(tile, i, j, layer);

        var tileIndex = GetTileIndex(tile, neighborCount);
        SpriteBatch.Draw(texture,
                position: destRect.Value.Position, scale: DrawScale,
                sourceRectangle: GetSourceRectangle(tileIndex, texture),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);
    }
}
