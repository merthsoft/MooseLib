using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

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

        public void Update(GameTime gameTime, OrthographicCamera camera)
        {
            Windows.RemoveAll(w => w.Close);
            CurrentMouseState = Mouse.GetState();
            foreach (var w in Windows)
            {
                var worldClick = camera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);

                var updateParams = new UpdateParameters(gameTime, worldClick - w.Position - w.Theme.ControlDrawOffset);

                if (w.Rectangle.Contains(worldClick))
                {
                    updateParams.MouseOver = true;
                    updateParams.LeftMouse = CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
                    updateParams.RightMouse = CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;
                }
                w.Update(updateParams);
            }
            PreviousMouseState = CurrentMouseState;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            => Windows.ForEach(w => w.Draw(gameTime, spriteBatch));

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
