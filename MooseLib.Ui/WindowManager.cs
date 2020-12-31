using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MooseLib.Ui.Controls;

namespace MooseLib.Ui
{
    public class WindowManager
    {
        public List<Theme> Themes { get; } = new();

        public int DefaultThemeIndex { get; set; }

        public Theme DefaultTheme => Themes[DefaultThemeIndex];

        public List<Window> Windows { get; } = new();

        internal MouseState CurrentMouseState { get; set; }

        internal MouseState PreviousMouseState { get; set; }

        public WindowManager(Theme theme)
            => Themes.Add(theme);

        public WindowManager(IEnumerable<Theme> themes)
            => Themes.AddRange(themes);

        public void Update(GameTime gameTime, MouseState currentMouseState, Vector2? worldMouse = null)
        {
            CurrentMouseState = currentMouseState;
            var mousePosition = worldMouse ?? new(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
            foreach (var w in Windows)
            {
                var updateParams = new UpdateParameters(gameTime, mousePosition - w.Position - w.Theme.ControlDrawOffset);

                if (w.Rectangle.Contains(mousePosition))
                {
                    updateParams.MouseOver = true;
                    updateParams.LeftMouse = CurrentMouseState.LeftButton.JustPressed(PreviousMouseState.LeftButton);
                    updateParams.RightMouse = CurrentMouseState.RightButton.JustPressed(PreviousMouseState.RightButton);
                }
                w.Update(updateParams);
            }
            Windows.RemoveAll(w => w.Close);
            PreviousMouseState = CurrentMouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
            => Windows.ForEach(w => w.Draw(spriteBatch));

        public Window NewWindow(int x, int y, int width, int height)
        {
            var ret = new Window(this, x, y, width, height);
            Windows.Add(ret);
            return ret;
        }

        public Window NewActionListWindow(int x, int y, Action<Control, UpdateParameters> action, params string[] options)
        {
            var ret = new Window(this, x, y, 0, 0);
            var list = ret.AddActionList(0, 0, action, options);
            ret.Size = list.CalculateSize() + new Vector2(DefaultTheme.TileWidth * 2, DefaultTheme.TileHeight);
            Windows.Add(ret);
            return ret;
        }
    }
}
