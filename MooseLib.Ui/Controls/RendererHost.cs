using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;
public class RendererHost : Control
{
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
        int layerNumber = 0;
        foreach (var(renderer, layer) in Renderers.Zip(Layers))
        {
            layer.DrawOffset += Position + parentOffset;
            renderer.Draw(MooseGame.Instance, gameTime, layer, layerNumber++);
            layer.DrawOffset -= Position + parentOffset;
        }
    }
}
