using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchTextureRenderer : SpriteBatchRenderer
{
    protected Texture2D SpriteSheet { get; }

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


    public override void Draw(MooseGame game, GameTime _gameTime, ILayer layer, Vector2 drawOffset)
    {
        if (layer is not ITileLayer<int> tileLayer)
            throw new Exception("TileLayer<int> layer expected");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
            {
                var tile = tileLayer.GetTileValue(i, j);
                if (tile >= 0)
                    DrawSprite(tile, i, j, tileLayer, drawOffset);
            }
    }

    public virtual void DrawSprite(int spriteIndex, int i, int j, ITileLayer<int> layer, Vector2 drawOffset, float layerDepth = 1f)
    {
        var destRect = GetDestinationRectangle(i, j, layer.DrawOffset + drawOffset);
        if (destRect != null)
            SpriteBatch.Draw(SpriteSheet, position: destRect.Value.Position,
                sourceRectangle: GetSourceRectangle(spriteIndex),
                color: Color, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, scale: DrawScale, layerDepth: layerDepth);
    }

    public RectangleF? GetDestinationRectangle(int i, int j, Vector2 drawOffset) 
        => new(i * TileWidth * DrawScale.X + drawOffset.X, 
               j * TileHeight * DrawScale.Y + drawOffset.Y, 
               TileWidth * DrawScale.X, TileHeight * DrawScale.Y);

    public Rectangle GetSourceRectangle(int spriteIndex, Texture2D? texture = null)
        => (texture ?? SpriteSheet).GetSourceRectangle(spriteIndex, TileWidth, TileHeight, TilePadding, TextureMargin);
}
