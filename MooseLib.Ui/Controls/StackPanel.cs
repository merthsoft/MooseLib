namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class StackPanel : FlowPanel, IControlContainer
{
    public StackDirection Direction { get; set; } = StackDirection.Vertical;
    public float Padding { get; set; }

    public StackPanel(IControlContainer container, float x, float y, float w, float h)
        : base(container, x, y, w, h) { }

    public override void Flow()
    {
        var x = 0f;
        var y = 0f;

        (var maxWidth, var maxHeight) = Controls.Aggregate((0f, 0f), (current, control) =>
        {
            if (control.Hidden)
                return current;
            var size = control.CalculateSize();
            return (MathF.Max(current.Item1, size.X), MathF.Max(current.Item2, size.Y));
        });

        foreach (var control in Controls)
        {
            if (control.Hidden)
                continue;
            if (Direction == StackDirection.Vertical)
            {
                var height = control.CalculateSize().Y;
                if (y + height > Height)
                {
                    x += maxWidth;
                    y = 0;
                }
                control.Position = new(x, y);
                y += height + Padding;
            }
            else
            {
                var width = control.CalculateSize().X;
                if (x + width > Width)
                {
                    y += maxHeight;
                    x = 0;
                }
                control.Position = new(x, y);
                x += width + Padding;
            }
        }
    }
}
