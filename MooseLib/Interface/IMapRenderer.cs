namespace Merthsoft.Moose.MooseEngine.Interface;

public interface IMapRenderer
{
    Vector2 DrawOffset { get; set; }
    Vector2 DrawScale { get; set; }

    void LoadContent(MooseContentManager contentManager);

    void Update(MooseGame game, GameTime gameTime, IMap map) { }

    bool PreDraw(MooseGame game, GameTime gameTime, IMap map);

    void Begin(Matrix transformMatrix) { }
    void Draw(MooseGame game, GameTime gameTime, IMap map);
    void End() { }
}