using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer;

public abstract class SpriteLayerBatchRenderer : ILayerRenderer
{
    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawScale { get; set; } = Vector2.One;

    public string? RenderKey { get; set; }
    public SpriteBatch SpriteBatch { get; set; }
    public Effect? Effect { get; set; }

    protected SpriteLayerBatchRenderer(SpriteBatch spriteBatch)
        => SpriteBatch = spriteBatch;

    public virtual bool PreDraw(MooseGame game, GameTime _gameTime, ILayer layer) => true;

    public virtual void Begin(Matrix viewMatrix)
        => SpriteBatch.Begin(
            SpriteSortMode.FrontToBack,
            BlendState.NonPremultiplied,
            SamplerState.PointClamp,
            effect: Effect,
            transformMatrix: viewMatrix);

    public abstract void Draw(MooseGame game, GameTime gameTime, ILayer layer);
    public virtual void Update(MooseGame game, GameTime gameTime, ILayer layer) { }
    public virtual void LoadContent(MooseContentManager contentManager) { }

    public virtual void End()
        => SpriteBatch.End();

    public void Dispose()
        => GC.SuppressFinalize(this);
}
