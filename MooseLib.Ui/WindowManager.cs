﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Merthsoft.Moose.MooseEngine.Ui;

public class WindowManager
{
    public GraphicsDevice GraphicsDevice { get; private set; }

    private readonly HashSet<Window> windowsToAdd = new();
    private HashSet<Window> windows = new();

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

    public Window[] Windows => windows.ToArray();

    internal MouseState CurrentMouseState { get; set; }

    internal MouseState PreviousMouseState { get; set; }

    internal KeyboardState CurrentKeyState { get; set; }

    internal KeyboardState PreviousKeyState { get; set; }

    private WindowManager(GraphicsDevice graphicsDevice)
        => GraphicsDevice = graphicsDevice;

    public WindowManager(GraphicsDevice graphicsDevice, Theme theme) : this(graphicsDevice)
        => AddTheme(theme);

    public WindowManager(GraphicsDevice graphicsDevice, IEnumerable<Theme> themes) : this(graphicsDevice)
        => themes.ForEach(AddTheme);

    public void Update(GameTime gameTime, bool gameHasFocus, MouseState currentMouseState, KeyboardState currentKeyState, Vector2? worldMouse = null)
    {
        CurrentMouseState = currentMouseState;
        CurrentKeyState = currentKeyState;
        var mousePosition = worldMouse ?? new(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        var windowFound = false;
        foreach (var w in Windows.Reverse())
            updateWindow(w);

        var newWindowSet = new HashSet<Window>(windows.Count + windowsToAdd.Count);
        foreach (var window in windows)
        {
            if (window.ShouldClose)
            {
                window.OnClose?.Invoke(window);
                window.Manager = null;
            }
            else
            {
                newWindowSet.Add(window);
            }
        }

        foreach (var window in windowsToAdd)
        {
            var hiddenCache = window.IsHidden;
            window.IsHidden = false;
            window.Manager = this;
            updateWindow(window);
            window.IsHidden = hiddenCache;

            newWindowSet.Add(window);
        }
        windowsToAdd.Clear();

        windows = newWindowSet;

        PreviousMouseState = CurrentMouseState;
        PreviousKeyState = CurrentKeyState;

        bool updateWindow(Window w)
        {
            UpdateParameters updateParams
                = new(gameTime, mousePosition - w.Position - w.Theme.TextureWindowControlDrawOffset, currentMouseState, currentKeyState);

            if (gameHasFocus && w.Rectangle.Contains(mousePosition) && !windowFound)
                windowFound = updateParams.MouseOver = true;

            updateParams.LeftMouseDown = CurrentMouseState.LeftButton == ButtonState.Pressed;
            updateParams.RightMouseDown = CurrentMouseState.RightButton == ButtonState.Pressed;
            updateParams.LeftMouseClick = CurrentMouseState.LeftButton.JustPressed(PreviousMouseState.LeftButton);
            updateParams.RightMouseClick = CurrentMouseState.RightButton.JustPressed(PreviousMouseState.RightButton);

            w.Update(updateParams);
            return windowFound;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var w in windows)
            w.Draw(spriteBatch);
    }

    public Window NewWindow(int x, int y, int width, int height, string? theme = null)
    {
        var ret = new Window(ThemeDictionary.GetValueOrDefault(theme) ?? DefaultTheme, x, y, width, height);
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
