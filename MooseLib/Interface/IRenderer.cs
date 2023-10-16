namespace Merthsoft.Moose.MooseEngine.Interface;

public interface IRenderer
{
    string? RenderKey { get; set; }

    void LoadContent(MooseContentManager contentManager);

    bool PreDraw(MooseGame game, GameTime gameTime);

    void Begin(Matrix transformMatrix);
    void Update(MooseGame game, GameTime gameTime);
    void Draw(MooseGame game, GameTime gameTime, Vector2 drawOffset);
    void End();
}
