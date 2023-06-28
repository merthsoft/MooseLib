using Merthsoft.Moose.Dungeon.Map;
using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.Dungeon.Ux;
public class MapPanel : Panel
{
    public Vector2 ZoomPosition = new(320, 30);
    public Vector2 NormalPosition = new(0, 475);

    public float Scale = 1f;

    private readonly Picture mapPicture;
    private readonly Picture mapCornerPicture;
    private readonly RendererHost rendererHost;

    public MapPanel(ILayerRenderer renderer, IControlContainer container, float x, float y) : base(container, x, y, 288, 288)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.None;
        
        mapPicture = this.AddPicture(0, 0, WiggleWizzardGame.Instance.MapTexture);
        rendererHost = this.AddControlPassThrough(new RendererHost(this, 0, 0));
        mapCornerPicture = this.AddPicture(0, 0, WiggleWizzardGame.Instance.MapCornerTexture);

        rendererHost.AddLayer(new MiniMapLayer("minimap", WiggleWizzardGame.Instance.DungeonSize, WiggleWizzardGame.Instance.DungeonSize), renderer);

        NormalPosition = Position;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset, GameTime gameTime)
    {
        mapPicture.Scale = new(Scale, Scale);
        mapCornerPicture.Scale = new(Scale, Scale);
        rendererHost.Scale = new(Scale, Scale);
        base.Draw(spriteBatch, parentOffset, gameTime);
    }

    public override void Update(UpdateParameters updateParameters) {
        var size = 288 * Scale;
        Size = new(size, size);
        base.Update(updateParameters);
    }
}
