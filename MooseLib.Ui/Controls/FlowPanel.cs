namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public abstract class FlowPanel : Panel
{
    public FlowPanel(Theme theme, float x, float y, float w, float h)
        : base(theme, x, y, w, h) { }

    public override void PreControlUpdate(UpdateParameters updateParameters)
    {
        if (Controls.Length > 0)
            Flow();
    }

    public abstract void Flow();
}
