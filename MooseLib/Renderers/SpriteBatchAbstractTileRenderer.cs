using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.Renderers;
public abstract class SpriteBatchAbstractTileRenderer : SpriteBatchRenderer
{
    protected Texture2D SpriteSheet { get; }

    public float Rotation { get; set; }
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    public int TileWidth { get; }
    public int TileHeight { get; }
    public int TextureMargin { get; }
    public int TilePadding { get; }


    public SpriteBatchAbstractTileRenderer(SpriteBatch spriteBatch, int tileWidth, int tileHeight, Texture2D sprites, int textureMargin = 0, int tilePadding = 0) : base(spriteBatch)
    {
        SpriteSheet = sprites;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        TextureMargin = textureMargin;
        TilePadding = tilePadding;
    }

    public virtual void DrawSprite(int spriteIndex, int i, int j, ILayer layer, Vector2 drawOffset, float layerDepth = 1f)
    {
        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset + drawOffset);
        if (destRect != null)
            SpriteBatch.Draw(SpriteSheet, position: destRect.Value.Position,
                sourceRectangle: GetSourceRectangle(spriteIndex),
                color: layer.DrawColor, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, scale: DrawScale, layerDepth: layerDepth);
    }

    public RectangleF? GetDestinationRectangle(int i, int j, Vector2 drawOffset)
        => new(i * TileWidth * DrawScale.X + drawOffset.X,
               j * TileHeight * DrawScale.Y + drawOffset.Y,
               TileWidth * DrawScale.X, TileHeight * DrawScale.Y);

    public Rectangle GetSourceRectangle(int spriteIndex, Texture2D? texture = null)
        => (texture ?? SpriteSheet).GetSourceRectangle(spriteIndex, TileWidth, TileHeight, TilePadding, TextureMargin);
}
