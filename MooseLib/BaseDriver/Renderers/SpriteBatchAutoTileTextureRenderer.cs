using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchAutoTileTextureRenderer : SpriteBatchTextureRenderer
{
    public Dictionary<int, Texture2D> AutoTileTextureMap { get; } = new();

    public Texture2D this[int index]
    {
        get => AutoTileTextureMap[index];
        set => AutoTileTextureMap[index] = value;
    }

    public SpriteBatchAutoTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) 
        : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        
    }

    protected virtual int GetTileIndex(int tile, int neighborCount)
        => neighborCount;

    protected virtual int CountNeighbors(int tile, int x, int y, ITileLayer<int> layer)
        => GetNeighborValue(tile, x +  0, y + -1, layer, 1) 
         + GetNeighborValue(tile, x + -1, y +  0, layer, 2) 
         + GetNeighborValue(tile, x +  1, y +  0, layer, 4) 
         + GetNeighborValue(tile, x +  0, y +  1, layer, 8);

    protected virtual int GetNeighborValue(int tile, int x, int y, ITileLayer<int> layer, int neighborValue)
    {
        if (x < 0 || y < 0 || x >= layer.Width || y >= layer.Height)
            return 0;

        if (Convert.ToInt32(layer.GetTileValue(x, y)) != tile)
            return 0;

        return neighborValue;
    }

    public override void DrawSprite(int spriteIndex, int i, int j, int layerNumber, ITileLayer<int> layer, float layerDepth = 1)
    { 
        var texture = AutoTileTextureMap.GetValueOrDefault(spriteIndex);

        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset);
        if (destRect == null)
            return;

        if (texture == null)
        {
                SpriteBatch.Draw(SpriteSheet,
                    destinationRectangle: destRect.Value,
                    sourceRectangle: GetSourceRectangle(spriteIndex),
                    color: Color, rotation: Rotation, effects: SpriteEffects,
                    origin: Vector2.Zero, layerDepth: layerDepth);
            return;
        }

        var neighborCount = CountNeighbors(spriteIndex, i, j, layer);

        var tileIndex = GetTileIndex(spriteIndex, neighborCount);
        SpriteBatch.Draw(texture,
                destinationRectangle: destRect.Value,
                sourceRectangle: GetSourceRectangle(tileIndex, texture),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);
    }
}
