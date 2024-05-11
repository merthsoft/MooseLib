namespace Merthsoft.Moose.MooseEngine.Ui.Controls;


public class GrowPanel : StackPanel
{
    public GrowPanel(IControlContainer container, float x, float y) : base(container, x, y, 0, 0)
    {

    }

    public override void Flow()
    {
        var x = 0f;
        var y = 0f;

        var width = 0f;
        var height = 0f;

        foreach (var control in Controls)
        {
            if (control.Hidden)
                continue;
            var size = control.CalculateSize();
            control.Position = new(x, y);

            if (Direction == StackDirection.Vertical)
            {
                var h = size.Y + Padding;
                y += h;
                height += h;
                if (size.X > width)
                    width = size.X;
            }
            else
            {
                var w = size.X + Padding;
                x += w;
                width += w;
                if (size.Y > height)
                    height = size.Y;
            }
        }

        if (Direction == StackDirection.Vertical)
            height += Padding;
        else
            width += Padding;

        var offset = Theme.GetDrawOffset(BackgroundDrawingMode);
        var newSize = new Vector2(width + offset.X, height + offset.Y+2);
        Size = BackgroundDrawingMode == BackgroundDrawingMode.Texture
                ? Theme.CalculateNewSize(newSize)
                : newSize;
    }
}
