using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public class Window
    {
        private Rectangle rectangle;

        public Theme Theme { get; set; }

        public bool ShouldClose { get; protected set; }

        public bool IsHidden { get; set; } = false;

        public bool DrawBackground { get; set; } = true;

        public Rectangle Rectangle {
            get => rectangle;
            set
            {
                var oldRect = rectangle;
                rectangle = value;
                RectangleChanged?.Invoke(this, new(oldRect, value));
            }
        }

        public Action<Window, ValueChangedParameters<Rectangle>>? RectangleChanged;

        public Vector2 Position
        {
            get => new(Rectangle.X, Rectangle.Y);
            set => Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);
        }

        public Vector2 Size
        {
            get => new(Rectangle.Width, Rectangle.Height);
            set => Rectangle = new Rectangle(Rectangle.X, Rectangle.Y, (int)value.X, (int)value.Y);
        }

        public int Width => Rectangle.Width;
        public int Height => Rectangle.Height;
        public int X => Rectangle.X;
        public int Y => Rectangle.Y;

        public List<Control> Controls { get; } = new();

        public Window(Rectangle rectangle, Theme theme)
            => (Rectangle, Theme)
             = (rectangle, theme);

        public Window(int x, int y, int w, int h, Theme theme)
            : this(new(x, y, w, h), theme) { }

        public virtual void Update(UpdateParameters updateParameters)
        {
            foreach (var c in Controls)
            {
                var controlUpdateParameters = new UpdateParameters(updateParameters.GameTime, updateParameters.LocalMousePosition - c.Position, updateParameters.GraphicsDevice);
                if (!IsHidden && !c.IsHidden && c.Rectangle.Contains(updateParameters.LocalMousePosition))
                {
                    controlUpdateParameters.MouseOver = true;
                    controlUpdateParameters.LeftMouse = updateParameters.LeftMouse;
                    controlUpdateParameters.RightMouse = updateParameters.RightMouse;
                }
                c.Update(controlUpdateParameters);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsHidden)
                return;

            if (DrawBackground)
                Theme.DrawWindowTexture(spriteBatch, Rectangle);

            foreach (var c in Controls)
                if (!c.IsHidden)
                    c.Draw(spriteBatch, Position + Theme.ControlDrawOffset);
        }

        public void Center(int width, int height)
            => Position = new(width / 2 - Width / 2, height / 2 - Height / 2);

        public void SetCellSize(int cellWidth, int cellHeight)
            => Size = new(cellWidth * Theme.TileWidth, cellHeight * Theme.TileHeight);

        public void SetCellPosition(int cellX, int cellY)
            => Position = new(cellX * Theme.TileWidth, cellY * Theme.TileHeight);

        public Vector2 MeasureString(string s, int fontIndex = 0)
            => Theme.Fonts[fontIndex].MeasureString(s);

        public void Close()
        {
            ShouldClose = true;
            IsHidden = true;
        }

        public void Hide()
            => IsHidden = true;

        public void Show()
            => IsHidden = false; 
        
        public Line AddLine(int x1, int y1, int x2, int y2, int thickness = 1)
             => Controls.AddPassThrough(new Line(this, x1, y1, x2, y2, thickness));

        public Label AddLabel(int x, int y, string text, int fontIndex = 0, Color? color = null, int strokeSize = 0, Color? strokeColor = null, bool hightlightOnHover = false, Color? highlightColor = null, bool forceHighlight = false)
            => Controls.AddPassThrough(new Label(this, x, y)
            {
                Text = text,
                FontIndex = fontIndex,
                Color = color,
                StrokeSize = strokeSize,
                StrokeColor = strokeColor ?? Theme.TextBorderColor,
                HighlightOnHover = hightlightOnHover || highlightColor.HasValue,
                HighlightColor = highlightColor,
                ForceHighlight = forceHighlight,
            });

        public Label AddActionLabel(int x, int y, string text, Action<Control, UpdateParameters> action)
            => Controls.AddPassThrough(new Label(this, x, y)
            {
                Text = text,
                Action = action,
            });

        public TextList AddActionList(int x, int y, int fontIndex, Action<Control, UpdateParameters> action, IEnumerable<string> options)
            => Controls.AddPassThrough(new TextList(this, x, y, options)
            {
                Action = action,
                SelectMode = SelectMode.None,
                FontIndex = fontIndex,
            });

        public TextGrid AddActionGrid(int x, int y, int gridWidth, int fontIndex, Action<Control, UpdateParameters> action, IEnumerable<string> options)
            => Controls.AddPassThrough(new TextGrid(this, x, y, gridWidth, options)
            {
                Action = action,
                SelectMode = SelectMode.None,
                FontIndex = fontIndex,
            });

        public Picture AddPicture(int x, int y, Texture2D texture, Rectangle? sourceRectangle = null)
            => Controls.AddPassThrough(new Picture(this, x, y, texture) { SourceRectangle = sourceRectangle ?? new(x, y, texture.Width, texture.Height) });

        public Picture AddPicture(int x, int y, Texture2D texture, Vector2 scale)
            => Controls.AddPassThrough(new Picture(this, x, y, texture) { Scale = scale });

        public Picture AddPicture(int x, int y, Texture2D texture, float scale)
            => Controls.AddPassThrough(new Picture(this, x, y, texture) { Scale = new(scale, scale) });
    }
}
