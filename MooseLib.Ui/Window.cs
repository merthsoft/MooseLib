using Merthsoft.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.MooseEngine.Ui
{
    public class Window
    {
        public Theme Theme { get; set; }

        public bool ShouldClose { get; protected set; }

        public bool Visible { get; set; } = true;
        
        public bool DrawBackground { get; set; } = true;

        public Rectangle Rectangle { get; set; }

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

        public virtual void Update(UpdateParameters updateParameters)
        {
            foreach (var c in Controls)
            {
                var controlUpdateParameters = new UpdateParameters(updateParameters.GameTime, updateParameters.LocalMousePosition - c.Position);
                if (Visible && c.Rectangle.Contains(updateParameters.LocalMousePosition))
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
            if (!Visible)
                return;

            if (DrawBackground)
                Theme.DrawWindowTexture(spriteBatch, Rectangle);

            Controls.ForEach(c => c.Draw(spriteBatch));
        }

        public void Center(int width, int height)
            => Position = new(width / 2 - Width / 2, height / 2 - Height / 2);

        public Line AddLine(int x1, int y1, int x2, int y2, int thickness = 1)
            => (Controls.AddPassThrough(new Line(this, x1, y1, x2, y2, thickness)) as Line)!;

        public Label AddLabel(int x, int y, string text, int fontIndex = 0)
            => (Controls.AddPassThrough(new Label(this, x, y)
            {
                Text = text,
                FontIndex = fontIndex,
            }) as Label)!;

        public Label AddActionLabel(int x, int y, string text, Action<Control, UpdateParameters> action)
            => (Controls.AddPassThrough(new Label(this, x, y)
            {
                Text = text,
                Action = action,
            }) as Label)!;

        public TextList AddActionList(int x, int y, int fontIndex, Action<Control, UpdateParameters> action, params string[] options)
            => (Controls.AddPassThrough(new TextList(this, x, y, options)
            {
                Action = action,
                SelectMode = SelectMode.None,
                FontIndex = fontIndex,
            }) as TextList)!;

        public TextList AddActionList(int x, int y, params (string text, Action<Control, UpdateParameters> action)[] options)
            => (Controls.AddPassThrough(new TextList(this, x, y, options.Select(o => o.text))
            {
                Action = (c, u) => options[(c as TextList)!.MouseOverIndex].action(c, u),
                SelectMode = SelectMode.None,
            }) as TextList)!;

        public Picture AddPicture(int x, int y, Texture2D texture, Vector2? scale = null)
            => (Controls.AddPassThrough(new Picture(this, x, y, texture)
            {
                Scale = scale ?? Vector2.One,
            }) as Picture)!;

        public void SetCellSize(int cellWidth, int cellHeight)
            => Size = new(cellWidth * Theme.TileWidth, cellHeight * Theme.TileHeight);

        public void SetCellPosition(int cellX, int cellY)
            => Position = new(cellX * Theme.TileWidth, cellY * Theme.TileHeight);

        public Vector2 MeasureString(string s, int fontIndex = 0)
            => Theme.Fonts[fontIndex].MeasureString(s);

        public void Close()
            => ShouldClose = true;

        public void Hide()
            => Visible = false;

        public void Show()
            => Visible = true;
    }
}
