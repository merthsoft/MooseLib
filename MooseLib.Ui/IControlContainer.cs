using Merthsoft.Moose.MooseEngine.Ui.Controls;

namespace Merthsoft.Moose.MooseEngine.Ui;

public interface IControlContainer
{
    Control[] Controls { get; }

    Window ParentWindow { get; }

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
