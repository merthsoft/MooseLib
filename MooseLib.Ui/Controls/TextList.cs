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
            => Horizontal
                ? new Vector2(Options.Count * SpacingOverride ?? Options.Max(o => Font.MeasureString(o.FormattedText).X) + Theme.TileDrawWidth, 0)
                : new Vector2(0, Options.Count * SpacingOverride ?? Theme.TileDrawHeight);


        public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        {
            if (Options.Count == 0 || Window.Theme.Fonts == null)
                return;

            var spacing = 0f;
            if (Horizontal)
                spacing = SpacingOverride ?? Options.Max(o => Font.MeasureString(o.FormattedText).X) + Theme.TileDrawWidth;
            else
                spacing = SpacingOverride ?? Theme.TileDrawHeight;

            for (var index = 0; index < Options.Count; index++)
            {
                var option = Options[index];
                var text = option.FormattedText;
                var color = Theme.ResolveTextColor(UpdateParameters, option.Enabled, option.Selected);
                var position = Position + parentOffset;

                if (Horizontal)
                    position += new Vector2(index * spacing, 0);
                else
                    position += new Vector2(0, index * spacing);

                 
                spriteBatch.DrawString(Font, text, position, color);
            }
        }
    }
}
