namespace Merthsoft.Moose.MooseEngine.Ui;

public interface IControlContainer
{
    Theme Theme { get; set; }
    Control[] Controls { get; }
    Control? FocusedControl { get;}
    
    IControlContainer ClearControls();
    IControlContainer AddControl(Control control);
    
    TControl GetControl<TControl>(int index) where TControl : Control;

    public Control? ResolveFocusedControl()
    {
        var c = FocusedControl;
        while (c != null)
        {
            var child = (c as IControlContainer)?.FocusedControl;
            if (child == null)
                return c;
            c = child;
        }

        return null;
    }
}
