using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui;

public class Window : IControlContainer
{
    internal MouseState CurrentMouseState { get; set; }

    internal MouseState PreviousMouseState { get; set; }

    internal KeyboardState CurrentKeyState { get; set; }

    internal KeyboardState PreviousKeyState { get; set; }

    private RectangleF rectangle;

    protected List<Control> controlsToAdd = new();
    protected List<Control> controls = new();
    public Control[] Controls => controls.ToArray();

    public Theme Theme { get; set; }

    public bool ShouldClose { get; protected set; }

    public bool Hidden { get; set; } = false;

    public BackgroundDrawingMode BackgroundDrawingMode { get; set; } = BackgroundDrawingMode.Texture;

    private Control? prompt;
    public Control? Prompt
    {
        get => prompt;
        set
        {
            prompt = value;
            if (prompt != null)
                FocusedControl = prompt;
        }
    }

    private Control? FocusedControl { get; set; }

    public RectangleF Rectangle
    {
        get => rectangle;
        set
        {
            var oldRect = rectangle;
            rectangle = value;
            RectangleChanged?.Invoke(this, new(oldRect, value));
        }
    }

    public Action<Window, ValueChangedParameters<RectangleF>>? RectangleChanged;

    public Vector2 Position
    {
        get => new(Rectangle.X, Rectangle.Y);
        set => Rectangle = Rectangle with { X = value.X, Y = value.Y };
    }

    public Vector2 Size
    {
        get => new(Rectangle.Width, Rectangle.Height);
        set => Rectangle = Rectangle with { Height = value.X, Width = value.Y };
    }

    public float Width => Rectangle.Width;
    public float Height => Rectangle.Height;
    public float X => Rectangle.X;
    public float Y => Rectangle.Y;

    public Window(Theme theme, float x, float y, float w = 0, float h = 0)
    {
        Theme = theme;
        Rectangle = new(x, y, w, h);
    }

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

    public void Update(GameTime gameTime, bool gameHasFocus, MouseState currentMouseState, KeyboardState currentKeyState, Vector2? worldMouse = null)
    {
        CurrentMouseState = currentMouseState;
        CurrentKeyState = currentKeyState;
        var mousePosition = worldMouse ?? new(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);

        var defaultUpdateParameters = new UpdateParameters(gameTime, Position + Theme.GetDrawOffset(BackgroundDrawingMode), mousePosition, currentMouseState, currentKeyState, FocusedControl);
        var windowUpdateParameters = defaultUpdateParameters with
        {
            MouseOver = Rectangle.Intersects(mousePosition),
            LeftMouseDown = CurrentMouseState.LeftButton == ButtonState.Pressed,
            RightMouseDown = CurrentMouseState.RightButton == ButtonState.Pressed,
            LeftMouseClick = CurrentMouseState.LeftButton.JustPressed(PreviousMouseState.LeftButton),
            RightMouseClick = CurrentMouseState.RightButton.JustPressed(PreviousMouseState.RightButton),
        };

        PreControlUpdate(windowUpdateParameters);
        controls.AddRange(controlsToAdd);
        controlsToAdd.Clear();

        if (!windowUpdateParameters.LeftMouseDown && !windowUpdateParameters.RightMouseDown)
            FocusedControl = null;

        if (Prompt != null)
            updateControl(Prompt);

        foreach (var c in Controls.Reverse())
            updateControl(c);

        if (Prompt != null && FocusedControl != Prompt && (windowUpdateParameters.LeftMouseClick || windowUpdateParameters.RightMouseClick))
            Prompt.Remove = true;

        if (Prompt?.Remove ?? false)
            Prompt = null;

        controls.RemoveAll(c => c.Remove);
        PostControlUpdate(windowUpdateParameters);

        PreviousMouseState = CurrentMouseState;
        PreviousKeyState = CurrentKeyState;

        void updateControl(Control c)
        {
            var updateParams = defaultUpdateParameters with
            {
                LocalMousePosition = mousePosition - c.Position - c.Theme.TextureWindowControlDrawOffset,
                FocusedControl = FocusedControl,
            };

            if (gameHasFocus && !Hidden
                && !c.Hidden && !c.Remove && c.Rectangle.Contains(mousePosition)
                && (FocusedControl == null || FocusedControl == c))
            {
                if (FocusedControl == null)
                    updateParams.FocusedControl = FocusedControl = c;
                updateParams.MouseOver = windowUpdateParameters.MouseOver;
                updateParams.LeftMouseDown = windowUpdateParameters.LeftMouseDown;
                updateParams.RightMouseDown = windowUpdateParameters.RightMouseDown;
                updateParams.LeftMouseClick = windowUpdateParameters.LeftMouseClick;
                updateParams.RightMouseClick = windowUpdateParameters.RightMouseClick;
            }

            c.Update(updateParams);
            if (updateParams.Prompt != null)
            {
                FocusedControl = Prompt = updateParams.Prompt;
                if (updateParams.CenterPrompt)
                    Prompt.Center(Width, Height);
            }

            c.ClearCompletedTweens();
        }
    }

    public virtual void PreControlUpdate(UpdateParameters updateParameters) { }
    public virtual void PostControlUpdate(UpdateParameters updateParameters) { }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Hidden)
            return;

        var pos = Position + Theme.DrawWindow(spriteBatch, Rectangle, BackgroundDrawingMode);

        foreach (var c in Controls)
            if (!c.Hidden && !c.Remove)
                c.Draw(spriteBatch, pos);

        Prompt?.Draw(spriteBatch, pos);
    }

    public void Center(float width, float height)
        => Position += new Vector2(width / 2 - Width / 2, height / 2 - Height / 2);

    public void Close()
    {
        ShouldClose = true;
        Hidden = true;
    }

    public void Hide()
        => Hidden = true;

    public void Show()
        => Hidden = false;
}
