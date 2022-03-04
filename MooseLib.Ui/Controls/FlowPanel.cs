namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public abstract class FlowPanel : Panel
{
    public FlowPanel(IControlContainer container, float x, float y, float w, float h)
        : base(container, x, y, w, h) { }

    public override void Update(UpdateParameters updateParameters)
    {
        base.Update(updateParameters);
        if (Controls.Length > 0)
            Flow();
    }

    public abstract void Flow();
}
