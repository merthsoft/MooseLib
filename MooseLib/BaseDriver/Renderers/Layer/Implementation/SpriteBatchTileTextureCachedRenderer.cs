using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;

public class SpriteBatchTileTextureCachedRenderer : SpriteLayerBatchRenderer
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
        DestinationRectangle = new Rectangle(0, 0, MapWidth * TileWidth, MapHeight * TileHeight);

        foreach (var layer in parentMap.Layers.Where(l => l is ITileLayer))
        {
            BackingTextures[layer] = null!;
        }
    }

    public override void LoadContent(MooseContentManager contentManager)
    {
        base.LoadContent(contentManager);
        foreach (var key in BackingTextures.Keys)
        {
            var target = new RenderTarget2D(SpriteBatch.GraphicsDevice, MapWidth * TileWidth, MapHeight * TileHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            SpriteBatch.GraphicsDevice.SetRenderTarget(target);
            SpriteBatch.GraphicsDevice.Clear(Color.Transparent);
            SpriteBatch.GraphicsDevice.SetRenderTarget(null);
            BackingTextures[key] = target;
        }
    }

    public override void Begin(Matrix viewMatrix)
    {
        ViewMatrix = viewMatrix;
    }

    public override bool PreDraw(MooseGame game, GameTime _gameTime, ILayer layer)
    {
        if (layer is not ITileLayer tileLayer)
            throw new Exception("TileLayer layer expected");

        if (layer.IsRenderDirty)
        {
            var target = BackingTextures[layer];

            SpriteBatch.GraphicsDevice.SetRenderTarget(target);
            //SpriteBatch.GraphicsDevice.Clear(Color.Pink);
            SpriteBatch.Begin(
                SpriteSortMode.Texture,
                BlendState.Opaque,
                SamplerState.PointClamp,
                effect: Effect);

            foreach (var (i, j) in tileLayer.RendererDirtyCells)
            {
                var tileValue = tileLayer.GetTileIndex(i, j);
                if (tileValue >= 0)
                    DrawSprite(tileValue, i, j, tileLayer, Vector2.Zero);

            }

            //for (int i = 0; i < tileLayer.Width; i++)
            //    for (int j = 0; j < tileLayer.Height; j++)
            //    {
            //        var tileValue = tileLayer.GetTileIndex(i, j);
            //        if (tileValue >= 0)
            //            DrawSprite(tileValue, i, j, tileLayer, Vector2.Zero);
            //    }

            layer.IsRenderDirty = false;
            SpriteBatch.End();
            SpriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        return true;
    }

    public override void Draw(MooseGame game, GameTime _gameTime, ILayer layer)
    {
        if (layer is not ITileLayer tileLayer)
            throw new Exception("TileLayer layer expected");

        SpriteBatch.Begin(
            SpriteSortMode.FrontToBack,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            effect: Effect,
            transformMatrix: ViewMatrix);

        var target = BackingTextures[layer];
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