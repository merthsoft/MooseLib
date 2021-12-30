using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class TextGrid : Control
    {
        public List<TextListOption> Options = new();

        public SelectMode SelectMode { get; set; } = SelectMode.None;
        public bool Horizontal { get; set; } = false;
        public float? SpacingOverride { get; set; } = null;

        public int MouseOverIndex { get; protected set; } = -1;
        public TextListOption? MouseOverOption => MouseOverIndex == -1 ? null : Options[MouseOverIndex];

        public int GridWidth { get; set; }

        public int? CellWidthOverride { get; set; } = null;
        public int? CellHeightOverride { get; set; } = null;

        public int CellWidth => CellWidthOverride ?? Window.Theme.TileDrawWidth;
        public int CellHeight => CellHeightOverride ?? Window.Theme.TileDrawHeight;

        public TextGrid(Window window, int x, int y, int gridWidth, IEnumerable<string> options) : base(window, x, y)
        {
            Options.AddRange(options.Select(o => new TextListOption(o)));
            GridWidth = gridWidth;
        }

        public override Vector2 CalculateSize()
            => new(GridWidth * CellWidth, (Options.Count / GridWidth + 1) * CellHeight);

        public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        {
            var position = Position + parentOffset;
            for (var index = 0; index < Options.Count; index++)
                DrawCell(spriteBatch, index, position);
        }

        protected virtual void DrawCell(SpriteBatch spriteBatch, int index, Vector2 drawOffset)
        {
            var x = (index % GridWidth) * CellWidth + (int)drawOffset.X;
            var y = (index / GridWidth) * CellHeight + (int)drawOffset.Y;

            var option = Options[index];
            var text = option.FormattedText;
            var color = option.Selected 
                        ? Theme.SelectedColor 
                        : option.Enabled
                            ? index == MouseOverIndex
                                ? Theme.TextMouseOverColor
                                : Theme.TextColor
                            : Theme.TextDisabledColor;

            spriteBatch.FillRectangle(x, y, CellWidth, CellHeight, Theme.ControlBackgroundColor);
            spriteBatch.DrawRectangle(x, y, CellWidth + 1, CellHeight + 1, Theme.ControlBorderColor);
            spriteBatch.DrawString(Font, text, new(x + 2, y + 1), color);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver)
            {
                var mouseX = (int)updateParameters.LocalMousePosition.X / CellWidth;
                var mouseY = (int)updateParameters.LocalMousePosition.Y / CellHeight;
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
}
