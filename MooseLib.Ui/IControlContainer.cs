using Merthsoft.Moose.MooseEngine.Ui.Controls;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public interface IControlContainer
    {
        Control[] Controls { get; }

        Window ParentWindow { get; }

        IControlContainer AddControl(Control control);
        
        TControl AddControlPassThrough<TControl>(TControl control) where TControl : Control
        {
            AddControl(control);
            return control;
        }
    }
}
