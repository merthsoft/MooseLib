namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ILayerRenderer : IDisposable
{
    void LoadContent(MooseContentManager contentManager);

    void Update(MooseGame game, GameTime gameTime) { }

    void Begin(Matrix transformMatrix) { }
    void Draw(MooseGame game, GameTime gameTime, ILayer layer, int layerNumber);
    void End() { }
}
