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

        public Label(Window window, int x, int y) : base(window, x, y)
        {
        }

        public override Vector2 CalculateSize()
            => StrokeSize == 0
                ? Window.Theme.Fonts[FontIndex].MeasureString(Text)
                : new(renderedTexture?.Width ?? 0, renderedTexture?.Height ?? 0);

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Text == null)
                return;

            var color = Color ?? (HighlightOnHover && UpdateParameters.MouseOver ? Theme.TextMouseOverColor : Theme.TextColor);

            if (strokeSize == 0)
            {
                spriteBatch.DrawString(Font, Text, GlobalPosition, color);
                return;
            }

            if (renderedTexture == null)
                renderedTexture = StrokeEffect.CreateStrokeSpriteFont(Font, Text, color, Vector2.One, StrokeSize, StrokeColor, spriteBatch.GraphicsDevice);

            spriteBatch.Draw(renderedTexture, GlobalPosition, Microsoft.Xna.Framework.Color.White);

        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (updateParameters.MouseOver && updateParameters.LeftMouse)
                Action?.Invoke(this, updateParameters);
            base.Update(updateParameters);
        }
    }
}
