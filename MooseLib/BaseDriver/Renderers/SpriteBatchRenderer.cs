using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public abstract class SpriteBatchRenderer : ILayerRenderer
{
    public string? RenderKey { get; set; }
    public SpriteBatch SpriteBatch { get; set; }

    protected SpriteBatchRenderer(SpriteBatch spriteBatch)
        => SpriteBatch = spriteBatch;

    public Effect? Effect { get; set; }

    public virtual void Begin(Matrix viewMatrix)
        => SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.NonPremultiplied,
            SamplerState.PointClamp,
            effect: Effect,
            transformMatrix: viewMatrix);

    public abstract void Draw(MooseGame game, GameTime gameTime, ILayer layer, int layerNumber);
    public virtual void Update(MooseGame game, GameTime gameTime) { }
    public virtual void LoadContent(MooseContentManager contentManager) { }

    public virtual void End()
        => SpriteBatch.End();

    public void Dispose()
        => GC.SuppressFinalize(this);
}
