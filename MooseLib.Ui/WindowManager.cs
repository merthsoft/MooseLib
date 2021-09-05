using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.Ui.Controls;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public class WindowManager
    {
        private readonly List<Window> windowsToAdd = new();
        private readonly List<Window> windows = new();

        public Dictionary<string, Theme> ThemeDictionary { get; } = new();

        public IReadOnlyCollection<Theme> Themes => ThemeDictionary.Values;

        private string defaultThemeName = null!; // Set in AddTheme called from ctor
        public string DefaultThemeName 
        { 
            get => defaultThemeName;
            set
            {
                defaultThemeName = value;
                defaultTheme = ThemeDictionary[value];
            }
        }

        private Theme? defaultTheme;
        public Theme DefaultTheme => defaultTheme ??= ThemeDictionary.GetValueOrDefault(DefaultThemeName) ?? Themes.First();

        public IReadOnlyCollection<Window> Windows => windows;

        internal MouseState CurrentMouseState { get; set; }

        internal MouseState PreviousMouseState { get; set; }

        public WindowManager(Theme theme)
            => AddTheme(theme);

        public WindowManager(IEnumerable<Theme> themes)
            => themes.ForEach(AddTheme);

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
            windows.RemoveAll(w => w.ShouldClose);
            windows.AddRange(windowsToAdd);
            windowsToAdd.Clear();
            PreviousMouseState = CurrentMouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
            => windows.ForEach(w => w.Draw(spriteBatch));

        public Window NewWindow(int x, int y, int width, int height, string? theme = null)
        {
            var ret = new Window(new(x, y, width, height), ThemeDictionary.GetValueOrDefault(theme) ?? DefaultTheme);
            windowsToAdd.Add(ret);
            return ret;
        }

        public Window NewActionListWindow(int x, int y, Action<Control, UpdateParameters> action, params string[] options)
        {
            var ret = new Window(new(x, y, 0, 0), DefaultTheme);
            var list = ret.AddActionList(0, 0, 0, action, options);
            ret.Size = list.CalculateSize() + new Vector2(DefaultTheme.TileWidth * 2, DefaultTheme.TileHeight);
            windowsToAdd.Add(ret);
            return ret;
        }

        public Window AddWindow(Window window)
        {
            windowsToAdd.Add(window);
            return window;
        }

        public void AddTheme(Theme theme)
        {
            ThemeDictionary[theme.Name] = theme;
            if (ThemeDictionary.Count == 1)
                DefaultThemeName = theme.Name;
        }
    }
}
