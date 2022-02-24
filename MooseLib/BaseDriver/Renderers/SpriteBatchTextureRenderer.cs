using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchTextureRenderer : SpriteBatchRenderer
{
    private Texture2D SpriteSheet { get; }

    public Color Color { get; set; } = Color.White;
    public float Rotation { get; set; }
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    public int TileWidth { get; }
    public int TileHeight { get; }
    public int TextureMargin { get; }
    public int TilePadding { get; }

    public SpriteBatchTextureRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites, int textureMargin = 0, int tilePadding = 0) : base(spriteBatch)
    {
        SpriteSheet = sprites;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        TextureMargin = textureMargin;
        TilePadding = tilePadding;
    }


    public override void Draw(GameTime _, ILayer layer, int layerNumber)
    {
        if (layer is not ITileLayer<int> tileLayer)
            throw new Exception("TileLayer<int> layer expected");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
                DrawSprite(tileLayer.GetTileValue(i, j), i, j, layerNumber, tileLayer);
    }

    public virtual void DrawSprite(int spriteIndex, int i, int j, int layerNumber, ILayer layer, float layerDepth = 1f)
        => SpriteBatch.Draw(SpriteSheet,
            destinationRectangle: GetDestinationRectangle(i, j, layer.DrawOffset),
            sourceRectangle: GetSourceRectangle(spriteIndex),
            color: Color, rotation: Rotation, effects: SpriteEffects,
            origin: Vector2.Zero, layerDepth: layerDepth);

    public Rectangle GetDestinationRectangle(int i, int j, Vector2 drawOffset)
        => new(i * TileWidth + (int)drawOffset.X, j * TileHeight + (int)drawOffset.Y, TileWidth, TileHeight);

    public Rectangle GetSourceRectangle(int spriteIndex)
    {
        var columns = SpriteSheet.Width / TileWidth;

        var sourceX = (spriteIndex % columns) * (TileWidth + TilePadding) + TextureMargin;
        var sourceY = (spriteIndex / columns) * (TileHeight + TilePadding) + TextureMargin;

        return new(sourceX, sourceY, TileWidth, TileHeight);
    }
}
