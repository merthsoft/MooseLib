using Merthsoft.Moose.MooseEngine.Ui.Controls;

namespace Merthsoft.Moose.MooseEngine.Ui;

public interface IControlContainer
{
    Control[] Controls { get; }

    Theme Theme { get; set; }

    IControlContainer ClearControls();
    IControlContainer AddControl(Control control);
}
