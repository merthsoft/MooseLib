using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class TextList : Control
    {
        public List<TextListOption> Options = new();

        public SelectMode SelectMode { get; set; } = SelectMode.None;
        public bool Horizontal { get; set; } = false;
        public float? SpacingOverride { get; set; } = null;

        public int MouseOverIndex { get; protected set; } = -1;
        public TextListOption? MouseOverOption => MouseOverIndex == -1 ? null : Options[MouseOverIndex];

        public TextList(Window window, int x, int y) : base(window, x, y)
        {
        }

        public TextList(Window window, int x, int y, IEnumerable<string> options) : this(window, x, y)
            => Options.AddRange(options.Select(o => new TextListOption(o)));

        public override Vector2 CalculateSize()
            => Options.Aggregate(Vector2.Zero, (acc, o) =>
        {
            var textSize = Window.Theme.Fonts[FontIndex].MeasureString(o.FormattedText);
            return new Vector2(Math.Max(acc.X, textSize.X), acc.Y + Window.Theme.TileHeight);
        });


        public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        {
            if (Options.Count == 0 || Window.Theme.Fonts == null)
                return;
            
            var font = Window.Theme.Fonts[FontIndex];

            var spacing = 0f;
            if (Horizontal)
                spacing = SpacingOverride ?? Options.Max(o => font.MeasureString(o.FormattedText).Y);
            else
                spacing = SpacingOverride ?? Window.Theme.TileHeight;

            for (var index = 0; index < Options.Count; index++)
            {
                var option = Options[index];
                var text = option.FormattedText;
                var color = option.Selected
                        ? Theme.SelectedColor
                        : option.Enabled
                            ? index == MouseOverIndex
                                ? Theme.TextMouseOverColor
                                : Theme.TextColor
                            : Theme.TextDisabledColor;
                var position = Position + parentOffset;

                if (Horizontal)
                    position += new Vector2(index * spacing, 0);
                else
                    position += new Vector2(0, index * spacing);

                 
                spriteBatch.DrawString(font, text, position, color);
            }
        }
    }
}
