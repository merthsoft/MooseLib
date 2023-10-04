namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ILayer
{

    int Width { get; }
    int Height { get; }

    string Name { get; }
    bool IsHidden { get; set; }
    bool IsRenderDirty { get; set; }
    Vector2 DrawOffset { get; set; }
    Vector2 DrawSize { get; set; }
    Color DrawColor { get; set; }

    string? RendererKey { get; }

    public void Update(GameTime gameTime);
}
