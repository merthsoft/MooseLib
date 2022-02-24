namespace Merthsoft.Moose.MooseEngine.Ui;

public interface IControlContainer
{
    Theme Theme { get; }
    Control[] Controls { get; }

    IControlContainer ClearControls();
    IControlContainer AddControl(Control control);
}
