namespace Merthsoft.Moose.MooseEngine.Ui.Controls;
public class StackPanel : FlowPanel
{
    public StackDirection Direction { get; set; } = StackDirection.Vertical;
    public float Padding { get; set; }

    public StackPanel(IControlContainer container, float x, float y, float w, float h)
        : base(container, x, y, w, h) { }

    public override void Flow()
    {
        var x = 0f;
        var y = 0f;

        var previousValue = 0f;

        foreach (var control in Controls)
        {
            if (control.Hidden)
                continue;
            var size = control.CalculateSize();
            if (Direction == StackDirection.Vertical)
            {
                var height = size.Y;
                if (y + height > Height)
                {
                    x += previousValue + Padding;
                    y = 0;
                }
                previousValue = size.X;
                control.Position = new(x, y);
                y += height + Padding;
            }
            else
            {
                var width = size.X;
                if (x + width > Width)
                {
                    y += previousValue + Padding;
                    x = 0;
                }
                previousValue = size.Y;
                control.Position = new(x, y);
                x += width + Padding;
            }
        }
    }
}
