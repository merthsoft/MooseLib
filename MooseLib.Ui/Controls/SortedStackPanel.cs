namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class SortedStackPanel<TControl, TKey> : StackPanel where TControl : Control
{
    private readonly Func<TControl, TKey> orderBy;

    public SortedStackPanel(Func<TControl, TKey> orderBy, IControlContainer container, float x, float y, float w, float h)
        : base(container, x, y, w, h) {
        this.orderBy = orderBy;
    }

    public override void Flow()
    {
        var x = 0f;
        var y = 0f;

        var previousValue = 0f;

        var sortedControls = Controls
                                .Cast<TControl>()
                                .OrderBy(orderBy)
                                .ToArray();
        controls.Clear();

        foreach (var control in sortedControls)
        {
            if (control.Hidden)
                continue;
            if (Direction == StackDirection.Vertical)
            {
                var size = control.CalculateSize();
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
                var size = control.CalculateSize();
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

        controls.AddRange(sortedControls);
    }
}
