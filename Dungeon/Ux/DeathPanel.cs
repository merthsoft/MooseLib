namespace Merthsoft.Moose.Dungeon.Ux;
public class DeathPanel : GrowPanel
{
    public DeathPanel(IControlContainer container, float x, float y) : base(container, x, y)
    {
    }

    public override void Update(UpdateParameters updateParameters)
    {
        ClearControls();
        BackgroundDrawingMode = BackgroundDrawingMode.Texture;
        this.AddLabel(" You died! ");
        base.Update(updateParameters);

    }
}
