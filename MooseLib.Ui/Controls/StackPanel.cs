using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class StackPanel : Panel, IControlContainer
    {
        public int ColumnWidth { get; set; }

        public StackPanel(Window window)
            : base(window)
        {
            DockMode = DockMode.Fill;
            Width = window.Width;
            Height = window.Height;
        }

        public StackPanel(Window window, int x, int y, int w, int h)
            : base(window, x, y, w, h)
        {
            DockMode = DockMode.None;
            Width = w;
            Height = h;
        }

        protected void Flow()
        {
            var x = 0;
            var y = 0;
            foreach (var control in Controls)
            {
                var height = (int)Math.Ceiling(control.CalculateSize().Y);
                if (y + height > Height)
                {
                    x += ColumnWidth;
                    y = 0;
                }
                control.Position = new(x, y);
                y += height;
            }
        }

        public override IControlContainer AddControl(Control control)
        {
            base.AddControl(control);
            var size = control.CalculateSize();
            if (size.X > ColumnWidth)
                ColumnWidth = (int)Math.Ceiling(size.X);
            Flow();
            return this;
        }

        public override void PreControlUpdate(UpdateParameters updateParameters)
            => Flow();
    }
}
