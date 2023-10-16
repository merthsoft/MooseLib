using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer;

public abstract class GraphicsDeviceLayerRenderer : ILayerRenderer
{
    public Vector2 DrawOffset { get; set; }
    public Vector2 DrawScale { get; set; } = Vector2.One;

    public string? RenderKey { get; set; }
    public GraphicsDevice GraphicsDevice { get; set; }
    public BasicEffect Effect { get; set; }

    protected Matrix ViewMatrix { get; set; } = Matrix.Identity;

    protected GraphicsDeviceLayerRenderer(GraphicsDevice graphicsDevice, BasicEffect effect)
    {
        GraphicsDevice = graphicsDevice;
        Effect = effect;
    }

    public virtual void Begin(Matrix viewMatrix)
        => ViewMatrix = viewMatrix;

    public abstract void Draw(MooseGame game, GameTime gameTime, ILayer layer);
    public virtual void Update(MooseGame game, GameTime gameTime, ILayer layer) { }
    public virtual void LoadContent(MooseContentManager contentManager) { }

    public virtual void End() { }

    public virtual bool PreDraw(MooseGame game, GameTime gameTime, ILayer layer) => true;
}

