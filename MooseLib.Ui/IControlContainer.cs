using Merthsoft.Moose.MooseEngine.Ui.Controls;

namespace Merthsoft.Moose.MooseEngine.Ui;

public interface IControlContainer
{
    Control[] Controls { get; }

    Theme Theme { get; set; }

    IControlContainer ClearControls();
    IControlContainer RemoveControl(Control control);
    IControlContainer RemoveControlAt(int index);

    IControlContainer AddControl(Control control);

    TControl AddControlPassThrough<TControl>(TControl control) where TControl : Control
    {
        AddControl(control);
        return control;
    }
}
