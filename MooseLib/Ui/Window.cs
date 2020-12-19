using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MooseLib.Ui
{
    public class Window
    {
        public WindowManager WindowManager { get; set; }
        public bool Close { get; set; }

        public Rectangle Rectangle { get; set; }

        public Vector2 Position
        {
            get => new(Rectangle.X, Rectangle.Y);
            set
            {
                Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);
            }
        }

        public Vector2 Size
        {
            get => new(Rectangle.Width, Rectangle.Height);
            set
            {
                Rectangle = new Rectangle(Rectangle.X, Rectangle.Y, (int)value.X, (int)value.Y);
            }
        }

        public List<Control> Controls { get; } = new();

        public Window(WindowManager windowManager, int x, int y, int w, int h)
            : this(windowManager, new(x, y, w, h)) { }

        public Window(WindowManager windowManager, Rectangle rectangle)
            => (WindowManager, Rectangle)
             = (windowManager, rectangle);

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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var numXTiles = Rectangle.Width / WindowManager.TileWidth;
            var numYTiles = Rectangle.Height / WindowManager.TileHeight;

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
                    WindowManager.DrawWindowTexture(spriteBatch, index, Position, x, y);
                }

            Controls.ForEach(c => c.Draw(spriteBatch));
        }

        public Label AddLabel(int x, int y, string text, Color color)
        {
            var ret = new Label(this)
            {
                Position = new(x, y),
                Text = text,
                Color = color
            };
            Controls.Add(ret);
            return ret;
        }

        public Label AddActionLabel(int x, int y, string text, Color color, Color mouseOverColor, Action<Control, UpdateParameters> action)
        {
            var ret = new Label(this)
            {
                Position = new(x, y),
                Text = text,
                Color = color,
                MouseOverColor = mouseOverColor,
                Action = action,
            };
            Controls.Add(ret);
            return ret;
        }
    }
}
