using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchTileTextureCachedRenderer : SpriteBatchRenderer
{
    protected Texture2D SpriteSheet { get; }

    public float Rotation { get; set; }
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    public int MapWidth { get; }
    public int MapHeight { get; }
    public int TileWidth { get; }
    public int TileHeight { get; }
    public int TextureMargin { get; }
    public int TilePadding { get; }

    private Matrix ViewMatrix;

    private Dictionary<ILayer, RenderTarget2D> BackingTextures = new(); // LoadContent
    private Rectangle DestinationRectangle;

    public SpriteBatchTileTextureCachedRenderer(SpriteBatch spriteBatch, IMap parentMap, Texture2D sprites, int textureMargin = 0, int tilePadding = 0) : base(spriteBatch)
    {
        SpriteSheet = sprites;
        TileWidth = parentMap.TileWidth;
        TileHeight = parentMap.TileHeight;
        TextureMargin = textureMargin;
        TilePadding = tilePadding;
        MapWidth = parentMap.Width;
        MapHeight = parentMap.Height;

        foreach (var layer in parentMap.Layers.Where(l => l is ITileLayer))
        {
            BackingTextures[layer] = null!;
        }

        DestinationRectangle = new Rectangle(0, 0, MapWidth * TileWidth, MapHeight * TileHeight);
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        foreach (var key in BackingTextures.Keys)
            BackingTextures[key] = new RenderTarget2D(contentManager.GraphicsDevice, MapWidth * TileWidth, MapHeight * TileHeight);
    }

    public override void Begin(Matrix viewMatrix)
    {
        ViewMatrix = viewMatrix;
    }

    public override void Draw(MooseGame game, GameTime _gameTime, ILayer layer, Vector2 drawOffset)
    {
        if (layer is not ITileLayer tileLayer)
            throw new Exception("TileLayer layer expected");

        var target = BackingTextures[layer];

        if (layer.IsRenderDirty)
        {
            SpriteBatch.Begin(
                SpriteSortMode.FrontToBack,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                effect: Effect);

            SpriteBatch.GraphicsDevice.SetRenderTarget(BackingTextures[layer]);
            SpriteBatch.GraphicsDevice.Clear(Color.Transparent);

            for (int i = 0; i < tileLayer.Width; i++)
                for (int j = 0; j < tileLayer.Height; j++)
                {
                    var tileValue = tileLayer.GetTileIndex(i, j);
                    if (tileValue >= 0)
                        DrawSprite(tileValue, i, j, tileLayer, drawOffset);
                }

            SpriteBatch.End();

            layer.IsRenderDirty = false;
            SpriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        SpriteBatch.Begin(
            SpriteSortMode.FrontToBack,
            BlendState.NonPremultiplied,
            SamplerState.PointClamp,
            effect: Effect,
            transformMatrix: ViewMatrix);

        SpriteBatch.Draw(target, 
                destinationRectangle: DestinationRectangle, 
                sourceRectangle: target.Bounds,
                color: layer.DrawColor, rotation: Rotation, effects: SpriteEffects,
                origin: Vector2.Zero, layerDepth: 1);

        SpriteBatch.End();
    }

    public override void End() { }

    public virtual void DrawSprite(int spriteIndex, int i, int j, ITileLayer layer, Vector2 drawOffset, float layerDepth = 1f)
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