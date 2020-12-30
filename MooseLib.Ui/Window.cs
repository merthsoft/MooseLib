using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MooseLib.Ui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MooseLib.Ui
{
    public class Window
    {
        public WindowManager WindowManager { get; set; }
        public Theme Theme { get; set; }

        public bool Close { get; set; }
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

        public List<Control> Controls { get; } = new();

        public Window(WindowManager windowManager, int x, int y, int w, int h)
            : this(windowManager, new(x, y, w, h)) { }

        public Window(WindowManager windowManager, Rectangle rectangle)
            => (WindowManager, Rectangle, Theme)
             = (windowManager, rectangle, windowManager.DefaultTheme);

        public void Update(UpdateParameters updateParameters)
        {
            foreach (var c in Controls)
            {
                var controlUpdateParameters = new UpdateParameters(updateParameters.GameTime, updateParameters.LocalMousePosition - c.Position);
                if (c.Rectangle.Contains(updateParameters.LocalMousePosition))
                {
                    controlUpdateParameters.MouseOver = true;
                    controlUpdateParameters.LeftMouse = updateParameters.LeftMouse;
                    controlUpdateParameters.RightMouse = updateParameters.RightMouse;
                }
                c.Update(controlUpdateParameters);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var numXTiles = Rectangle.Width / Theme.TileWidth;
            var numYTiles = Rectangle.Height / Theme.TileHeight;

            for (var x = 0; x < numXTiles; x++)
                for (var y = 0; y < numYTiles; y++)
                {
                    var index = 4;
                    if ((x, y) == (0, 0))
                        index = 0;
                    else if ((x, y) == (0, numYTiles - 1))
                        index = 6;
                    else if (x == 0)
                        index = 3;
                    else if ((x, y) == (numXTiles - 1, 0))
                        index = 2;
                    else if (y == 0)
                        index = 1;
                    else if ((x, y) == (numXTiles - 1, numYTiles - 1))
                        index = 8;
                    else if (y == numYTiles - 1)
                        index = 7;
                    else if (x == numXTiles - 1)
                        index = 5;
                    Theme.DrawWindowTexture(spriteBatch, index, Position, x, y);
                }

            Controls.ForEach(c => c.Draw(spriteBatch));
        }

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

        public TextList AddActionList(int x, int y, Action<Control, UpdateParameters> action, params string[] options)
            => (Controls.AddPassThrough(new TextList(this, x, y, options)
            {
                Action = action,
                SelectMode = SelectMode.None,
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
    }
}
