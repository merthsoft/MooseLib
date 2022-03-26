namespace Merthsoft.Moose.MooseEngine.Defs;

public record TextureGameObjectDef(string DefName, string TextureName, Rectangle? SourceRectangle = null) : GameObjectDef(DefName)
{
    public Texture2D Texture { get; private set; } = null!;

    public override void LoadContent(MooseContentManager contentManager)
        => Texture = contentManager.LoadImage(TextureName);
}
