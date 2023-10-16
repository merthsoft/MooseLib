using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;
public class RendererHost : Control
{
    public Vector2 Scale { get; set; }

    public List<ILayerRenderer> Renderers = new();
    public List<ILayer> Layers = new();

    public RendererHost(IControlContainer container, float x, float y) : base(container, x, y)
    {
    }

    public void AddLayer(ILayer layer, ILayerRenderer renderer)
    {
        Layers.Add(layer);
        Renderers.Add(renderer);
    }

    public override Vector2 CalculateSize() => Vector2.One;
    
    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset, GameTime gameTime)
    {
        foreach (var(renderer, layer) in Renderers.Zip(Layers))
        {
            renderer.DrawScale = Scale;
            renderer.DrawOffset = parentOffset;
            renderer.Draw(MooseGame.Instance, gameTime, layer);
        }
    }

    public override void Update(UpdateParameters updateParameters)
    {
        foreach (var (renderer, layer) in Renderers.Zip(Layers))
            renderer.Update(MooseGame.Instance, updateParameters.GameTime, layer);
    }
}
