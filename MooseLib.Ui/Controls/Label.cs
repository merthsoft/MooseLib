using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class Label : Control
    {
        private string? text;
        public string? Text
        {
            get => text;
            set
            {
                text = value;
                renderedTexture = null;
            }
        }

        private int strokeSize;
        public int StrokeSize { 
            get => strokeSize;
            set
            {
                strokeSize = value;
                renderedTexture = null;
            }
        }
        
        private Color strokeColor;
        public Color StrokeColor
        {
            get => strokeColor;
            set
            {
                strokeColor = value;
                renderedTexture = null;
            }
        }

        public bool HighlightOnHover { get; set; }

        private Texture2D? renderedTexture;

        public Color? TextColor { get; set; }
        public Color ResolvedTextColor => TextColor ?? Theme.ResolveTextColor(UpdateParameters, Enabled, false, HighlightOnHover);

        public Label(Window window, int x, int y) : base(window, x, y)
        {
        }

        public override Vector2 CalculateSize()
            => StrokeSize == 0
                ? Window.Theme.Fonts[FontIndex].MeasureString(Text)
                : new(renderedTexture?.Width ?? 0, renderedTexture?.Height ?? 0);

        public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        {
            if (Text == null)
                return;

            var position = Position + parentOffset;

            if (strokeSize == 0)
                spriteBatch.DrawString(Font, Text, position, ResolvedTextColor);
            else
                spriteBatch.Draw(renderedTexture, position, Microsoft.Xna.Framework.Color.White);

        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouseClick)
                Action?.Invoke(this, updateParameters);

            if (renderedTexture == null && StrokeSize > 0)
                renderedTexture = StrokeEffect.CreateStrokeSpriteFont(Font, Text, ResolvedTextColor, Vector2.One, StrokeSize, StrokeColor, Window.GraphicsDevice);

            base.Update(updateParameters);
        }
    }
}
