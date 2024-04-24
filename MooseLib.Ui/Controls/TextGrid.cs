namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class TextGrid : Control
{
    public List<Option> Options = [];

    public SelectMode SelectMode { get; set; } = SelectMode.None;
    public bool Horizontal { get; set; } = false;
    public float? SpacingOverride { get; set; } = null;

    public int MouseOverIndex { get; protected set; } = -1;
    public Option? MouseOverOption => MouseOverIndex == -1 ? null : Options[MouseOverIndex];

    public int GridWidth { get; set; }

    public int? CellWidthOverride { get; set; } = null;
    public int? CellHeightOverride { get; set; } = null;

    public int CellWidth => CellWidthOverride ?? Theme.TileDrawWidth;
    public int CellHeight => CellHeightOverride ?? Theme.TileDrawHeight;

    public TextGrid(IControlContainer container, float x, float y, int gridWidth, IEnumerable<string> options) : base(container, x, y)
    {
        Options.AddRange(options.Select(o => new Option(o)));
        GridWidth = gridWidth;
    }

    public override Vector2 CalculateSize()
        => new(GridWidth * CellWidth + 1, ((Options.Count == 0 ? 1 : Options.Count) / GridWidth + 1) * CellHeight + 1);

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset, GameTime gameTime)
    {
        var position = Position + parentOffset;
        for (var index = 0; index < Options.Count; index++)
            DrawCell(spriteBatch, index, MouseOverIndex, position);
    }

    protected virtual void DrawCell(SpriteBatch spriteBatch, int index, int mouseIndex, Vector2 drawOffset)
    {
        var x = (index % GridWidth) * CellWidth + (int)drawOffset.X;
        var y = (index / GridWidth) * CellHeight + (int)drawOffset.Y;

        var option = Options[index];
        var text = option.FormattedText;
        var textSize = Font.MeasureString(text);

        var cellUpdateParams = UpdateParameters with { MouseOver = index == mouseIndex };

        spriteBatch.FillRectangle(x, y, CellWidth, CellHeight, Theme.ResolveBackgroundColor(cellUpdateParams, Enabled));
        spriteBatch.DrawRectangle(x, y, CellWidth + 1, CellHeight + 1, Theme.ControlBorderColor);

        var color = Theme.ResolveTextColor(cellUpdateParams, option.Enabled, option.Selected);
        spriteBatch.DrawString(Font, text, new(x + 2 + CellWidth / 2 - textSize.X / 2, y + CellHeight / 2 - textSize.Y / 2), color);
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (updateParameters.MouseOver)
        {
            var mouseX = (int)(updateParameters.LocalMousePosition.X / CellWidth);
            var mouseY = (int)(updateParameters.LocalMousePosition.Y / CellHeight);
            MouseOverIndex = mouseY * GridWidth + mouseX;
            if (updateParameters.MouseOver && updateParameters.LeftMouseClick && MouseOverIndex < Options.Count)
            {
                if (SelectMode == SelectMode.Single)
                    Options.ForEach(o => o.Selected = false);
                Options[MouseOverIndex].Selected = SelectMode switch
                {
                    SelectMode.Multiple => !Options[MouseOverIndex].Selected,
                    SelectMode.Single => true,
                    SelectMode.None => false,
                    _ => false,
                };
                Action?.Invoke(this, updateParameters);
            }
        }
        else
            MouseOverIndex = -1;
        base.Update(updateParameters);
    }
}
