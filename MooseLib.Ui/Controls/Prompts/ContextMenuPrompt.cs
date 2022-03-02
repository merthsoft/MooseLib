namespace Merthsoft.Moose.MooseEngine.Ui.Controls.Prompts;
public class ContextMenuPrompt : StackPanel
{
    public ContextMenuPrompt(IList<string> items, Action<string> itemSelected, IControlContainer container, float x, float y) : base(container, x, y, 0, 0)
    {
        var longestString = 0f;
        foreach (var item in items)
        {
            this.AddActionLabel(0, 0, item, (_, __) =>
            {
                Remove = true;
                itemSelected(item);
            });
            var width = MeasureString(item).X;
            if (width > longestString)
                longestString = width;
        }

        BackgroundDrawingMode = BackgroundDrawingMode.Basic;
        var drawSize = Theme.GetDrawOffset(BackgroundDrawingMode) * 2;
        Size = new(longestString.Ceiling() + drawSize.X,
            (Theme.MeasureString("X", 0).Y * items.Count).Ceiling() + drawSize.Y);
    }
}
