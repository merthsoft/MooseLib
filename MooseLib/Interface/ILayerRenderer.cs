namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ILayerRenderer : IDisposable
{
    Vector2 DrawOffset { get; set; }
    Vector2 DrawScale { get; set; }

    void LoadContent(MooseContentManager contentManager);

    void Update(MooseGame game, GameTime gameTime) { }

    void Begin(Matrix transformMatrix) { }
    void Draw(MooseGame game, GameTime gameTime, ILayer layer, Vector2 drawOffset);
    void End() { }
}
