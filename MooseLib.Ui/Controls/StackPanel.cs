namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class StackPanel : FlowPanel, IControlContainer
    {
        public StackDirection Direction { get; set; } = StackDirection.Vertical;
        public int Padding { get; set; }

        public StackPanel(Window window)
            : base(window) { }
        public StackPanel(Window window, int x, int y, int w, int h)
            : base(window, x, y, w, h) { }

        protected override void Flow()
        {
            var x = 0;
            var y = 0;

            (var maxWidth, var maxHeight) = Controls.Aggregate((0, 0), (current, control) =>
            {
                var size = control.CalculateSize();
                return (MathF.Max(current.Item1, size.X).Ceiling(), MathF.Max(current.Item2, size.Y).Ceiling());
            });

            foreach (var control in Controls)
            {
                if (Direction == StackDirection.Vertical)
                {
                    var height = (int)Math.Ceiling(control.CalculateSize().Y);
                    if (y + height > Height)
                    {
                        x += maxWidth;
                        y = 0;
                    }
                    control.Position = new(x, y);
                    y += height + Padding;
                } else
                {
                    var width = (int)Math.Ceiling(control.CalculateSize().X);
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
}
