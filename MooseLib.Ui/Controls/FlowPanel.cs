namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public abstract class FlowPanel : Panel
{
    public FlowPanel(Theme theme, int x, int y, int w, int h)
        : base(theme, x, y, w, h) { }

    public override void PreControlUpdate(UpdateParameters updateParameters)
    {
        if (Controls.Length > 0)
            Flow();
    }

    protected abstract void Flow();
}
