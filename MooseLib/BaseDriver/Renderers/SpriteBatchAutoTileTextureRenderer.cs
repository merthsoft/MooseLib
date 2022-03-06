using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchAutoTileTextureRenderer : SpriteBatchTextureRenderer
{
    public Dictionary<int, Texture2D> AutoTileTextureMap { get; } = new();

    public SpriteBatchAutoTileTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D baseTexture, int textureMargin = 0, int tilePadding = 0) : base(spriteBatch, tileWidth, tileHeight, baseTexture, textureMargin, tilePadding)
    {
        
    }


    public override void Draw(GameTime _, ILayer layer, int layerNumber)
    {
        if (layer is not ITileLayer<int> tileLayer)
            throw new Exception("TileLayer<int> layer expected");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
            {
                var tile = tileLayer.GetTileValue(i, j);
                if (tile >= 0)
                    DrawSprite(tile, i, j, layerNumber, tileLayer);
            }
    }

    protected virtual int GetTileIndex(int neighborCount)
        => neighborCount;

    public virtual void DrawSprite(int spriteIndex, int i, int j, int layerNumber, ITileLayer<int> layer, float layerDepth = 0f)
    {
        var texture = AutoTileTextureMap.GetValueOrDefault(spriteIndex);
        if (texture == null)
        {
            SpriteBatch.Draw(SpriteSheet,
                destinationRectangle: GetDestinationRectangle(i, j, layer.DrawOffset),
                sourceRectangle: GetSourceRectangle(spriteIndex),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);
            return;
        }

        var neighborCount = countNeighbor(0, -1, 1)
                          + countNeighbor(-1, 0, 2)
                          + countNeighbor(1, 0, 4)
                          + countNeighbor(0, 1, 8);
        

        var tileIndex = GetTileIndex(neighborCount);
        SpriteBatch.Draw(texture,
                destinationRectangle: GetDestinationRectangle(i, j, layer.DrawOffset),
                sourceRectangle: GetSourceRectangle(tileIndex, texture),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: layerDepth);

        int countNeighbor(int x, int y, int countValue)
        {
            if (x == 0 && y == 0)
                return 0;
            var newX = x + i;
            var newY = y + j;
            return newX >= 0 && newY >= 0 && newX < layer.Width && newY < layer.Height
                ? layer.GetTileValue(newX, newY) == spriteIndex 
                    ? countValue 
                    : 0
                : 0;
        }
    }
}
