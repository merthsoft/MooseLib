namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class ToggleButton : Button
{
    public bool Toggled { get; set; } = false;

    protected override Color ResolvedTextColor => Theme.ResolveTextColor(UpdateParameters, Enabled, Toggled);

    public ToggleButton(string text, IControlContainer container, float x, float y) : base(text, container, x, y) { }

    public override void Update(UpdateParameters updateParameters)
    {
        if (updateParameters.MouseOver && (updateParameters.LeftMouseClick || updateParameters.RightMouseClick))
        {
            Toggled = !Toggled;
            Action?.Invoke(this, updateParameters);
        }
    }
}
