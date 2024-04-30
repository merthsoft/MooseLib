using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Map;

public abstract class SpriteBatchMapRenderer(SpriteBatch spriteBatch) : IMapRenderer
{
    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawScale { get; set; } = Vector2.One;

    public SpriteBatch SpriteBatch { get; set; } = spriteBatch;
    public Effect? Effect { get; set; }

    public virtual bool PreDraw(MooseGame game, GameTime _gameTime, IMap map) => true;

    public virtual void Begin(Matrix viewMatrix)
        => SpriteBatch.Begin(
            SpriteSortMode.FrontToBack,
            BlendState.NonPremultiplied,
            SamplerState.PointClamp,
            effect: Effect,
            transformMatrix: viewMatrix);

    public abstract void Draw(MooseGame game, GameTime gameTime, IMap map);
    public virtual void Update(MooseGame game, GameTime gameTime) { }
    public virtual void LoadContent(MooseContentManager contentManager) { }

    public virtual void End()
        => SpriteBatch.End();

    public void Dispose()
        => GC.SuppressFinalize(this);
}
