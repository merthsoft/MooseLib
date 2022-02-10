using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public class Window : IControlContainer
    {
        private Rectangle rectangle;

        protected List<Control> controlsToAdd = new();
        protected List<Control> controls = new();
        public Control[] Controls => controls.ToArray();

        public Window ParentWindow => this;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public Theme Theme { get; set; }

        public bool ShouldClose { get; protected set; }

        public bool IsHidden { get; set; } = false;

        public bool DrawBackground { get; set; } = true;

        public Action<Window>? OnClose { get; set; }

        public WindowManager? Manager { get; set; }

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

        public Window(GraphicsDevice graphicsDevice, Rectangle rectangle, Theme theme)
            => (Rectangle, Theme, GraphicsDevice)
             = (rectangle, theme, graphicsDevice);

        public Window(GraphicsDevice graphicsDevice, int x, int y, int w, int h, Theme theme)
            : this(graphicsDevice, new(x, y, w, h), theme) { }

        public virtual IControlContainer AddControl(Control control)
        {
            controlsToAdd.Add(control);
            return this;
        }

        public virtual IControlContainer ClearControls()
        {
            controls.Clear();
            controlsToAdd.Clear();
            return this;
        }

        public virtual IControlContainer RemoveControl(Control control)
        {
            controls.Remove(control);
            controlsToAdd.Remove(control);
            return this;
        }

        public virtual IControlContainer RemoveControlAt(int index)
        {
            controls.RemoveAt(index);
            return this;
        }

        public virtual void Update(UpdateParameters updateParameters)
        {
            PreControlUpdate(updateParameters);
            foreach (var c in Controls)
            {
                UpdateParameters controlUpdateParameters 
                    = new (updateParameters.GameTime, updateParameters.LocalMousePosition - c.Position, updateParameters.RawMouseState, updateParameters.RawKeyState);
                if (c.Rectangle.Contains(updateParameters.LocalMousePosition) && !IsHidden && !c.IsHidden && updateParameters.MouseOver)
                {
                    controlUpdateParameters.MouseOver = true;
                    controlUpdateParameters.LeftMouseClick = updateParameters.LeftMouseClick;
                    controlUpdateParameters.RightMouseClick = updateParameters.RightMouseClick;
                    controlUpdateParameters.LeftMouseDown = updateParameters.LeftMouseDown;
                    controlUpdateParameters.RightMouseDown = updateParameters.RightMouseDown;
                }
                c.Update(controlUpdateParameters);
                c.UpdateParameters = controlUpdateParameters;
            }
            controls.AddRange(controlsToAdd);
            controlsToAdd.Clear();
            PostControlUpdate(updateParameters);
        }

        public virtual void PreControlUpdate(UpdateParameters updateParameters) { }
        public virtual void PostControlUpdate(UpdateParameters updateParameters) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsHidden)
                return;

            if (DrawBackground)
                Theme.DrawWindowTexture(spriteBatch, Rectangle);

            foreach (var c in Controls)
                if (!c.IsHidden)
                    c.Draw(spriteBatch, Position + (DrawBackground ? Theme.ControlDrawOffset : Vector2.Zero));
        }

        public void Center(int width, int height)
            => Position += new Vector2(width / 2 - Width / 2, height / 2 - Height / 2);

        public void Center(Vector2 size)
            => Position += new Vector2(size.X.Ceiling() / 2 - Width / 2, size.Y.Ceiling() / 2 - Height / 2);

        public void SetCellSize(int cellWidth, int cellHeight)
            => Size = new(cellWidth * Theme.TileWidth, cellHeight * Theme.TileHeight);

        public void SetCellPosition(int cellX, int cellY)
            => Position = new(cellX * Theme.TileWidth, cellY * Theme.TileHeight);

        public Vector2 MeasureString(string s, int fontIndex = 0)
            => Theme.Fonts[fontIndex].MeasureString(s);

        public string TruncateString(string s, int width, string truncationString = "...", int fontIndex = 0)
        {
            var totalLength = ParentWindow.MeasureString(s, fontIndex).X;
            if (totalLength < width)
                return s;

            var length = ParentWindow.MeasureString(truncationString, fontIndex).X;
            var nameBuilder = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                nameBuilder.Append(c);
                length += ParentWindow.MeasureString(c.ToString(), fontIndex).X;
                if (length >= width)
                    break;
            }
            return nameBuilder.Append(truncationString).ToString();
        }

        public void Close()
        {
            ShouldClose = true;
            IsHidden = true;
        }

        public void Hide()
            => IsHidden = true;

        public void Show()
            => IsHidden = false;
    }
}
