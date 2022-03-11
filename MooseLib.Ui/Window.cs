using Merthsoft.Moose.MooseEngine.Interface;
using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.MooseEngine.Ui;

public class Window : IControlContainer, ITweenOwner
{
    internal MouseState CurrentMouseState { get; set; }

    internal MouseState PreviousMouseState { get; set; }

    internal KeyboardState CurrentKeyState { get; set; }

    internal KeyboardState PreviousKeyState { get; set; }

    private RectangleF rectangle;

    protected List<Control> controls = new();
    public Control[] Controls => controls.ToArray();
    public TControl GetControl<TControl>(int index) where TControl : Control
        => (TControl)controls[index];

    public Theme Theme { get; set; }
    public OrthographicCamera? Camera { get; set; }

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

    public List<Tween> ActiveTweens { get; } = new();

    private Color overlayColor;
    public Color OverlayColor
    {
        get => overlayColor;
        set
        {
            overlayColor = value;
            foreach (var t in ActiveTweens.Where(t => t.MemberName == "OverlayColor"))
                t.Cancel();
        }
    }

    public float OverlayAlpha
    {
        get => OverlayColor.A / 255f;
        set => OverlayColor = new Color(OverlayColor, value);
    }

    public Window(Theme theme, float x, float y, float w = 0, float h = 0)
    {
        Theme = theme;
        Rectangle = new(x, y, w, h);
        OverlayColor = Color.Transparent;
    }

    public virtual IControlContainer AddControl(Control control)
    {
        controls.Add(control);
        return this;
    }

    public virtual IControlContainer ClearControls()
    {
        foreach (var control in controls)
            control.Remove = true;

        return this;
    }

    public void Update(GameTime gameTime)
    {
        CurrentMouseState = Mouse.GetState();
        CurrentKeyState = Keyboard.GetState();
        var gameHasFocus = MooseGame.Instance.IsActive;
        var mousePosition = Camera?.ScreenToWorld(CurrentMouseState.Position.ToVector2())
            ?? new(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);

        var defaultUpdateParameters = new UpdateParameters(gameTime,
            Position + Theme.GetDrawOffset(BackgroundDrawingMode),
            mousePosition,
            CurrentMouseState,
            CurrentKeyState,
            FocusedControl);
        var windowUpdateParameters = defaultUpdateParameters with
        {
            MouseOver = Rectangle.Intersects(mousePosition),
            LeftMouseDown = CurrentMouseState.LeftButton == ButtonState.Pressed,
            RightMouseDown = CurrentMouseState.RightButton == ButtonState.Pressed,
            LeftMouseClick = CurrentMouseState.LeftButton.JustPressed(PreviousMouseState.LeftButton),
            RightMouseClick = CurrentMouseState.RightButton.JustPressed(PreviousMouseState.RightButton),
        };

        PreControlUpdate(windowUpdateParameters);
        
        if (!windowUpdateParameters.LeftMouseDown && !windowUpdateParameters.RightMouseDown)
            FocusedControl = null;

        if (Prompt != null)
            updateControl(Prompt);

        for (var i = Controls.Length - 1; i >= 0 && i < Controls.Length; i--)
            updateControl(Controls[i]);

        if (Prompt != null && FocusedControl != Prompt && (windowUpdateParameters.LeftMouseClick || windowUpdateParameters.RightMouseClick))
            Prompt.Remove = true;

        if (Prompt?.Remove ?? false)
            Prompt = null;

        foreach (var control in controls)
            if (control.Remove && control is IDisposable d)
                d.Dispose();

        controls.RemoveAll(c => c.Remove);

        PostControlUpdate(windowUpdateParameters);

        PreviousMouseState = CurrentMouseState;
        PreviousKeyState = CurrentKeyState;

        void updateControl(Control c)
        {
            if (c.Remove)
                return;

            var updateParams = defaultUpdateParameters with
            {
                LocalMousePosition = mousePosition - c.Position - c.Theme.GetDrawOffset(BackgroundDrawingMode),
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
            c.UpdateParameters = updateParams;
            if (updateParams.Prompt != null)
            {
                FocusedControl = Prompt = updateParams.Prompt;
                if (updateParams.CenterPrompt)
                    Prompt.Center(Width, Height);
            }
            c.PostUpdate();
        }
    }

    public virtual void PreControlUpdate(UpdateParameters updateParameters) { }
    public virtual void PostControlUpdate(UpdateParameters updateParameters) { }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (Hidden)
            return;

        var pos = Position + Theme.DrawWindow(spriteBatch, Rectangle, BackgroundDrawingMode);

        foreach (var c in Controls)
            if (!c.Hidden)
                c.Draw(spriteBatch, pos, gameTime);

        Prompt?.Draw(spriteBatch, pos, gameTime);

        if (Theme.Cursor != null)
        {
            var mousePosition = Camera?.ScreenToWorld(CurrentMouseState.Position.ToVector2())
                ?? new(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
            spriteBatch.Draw(Theme.Cursor, mousePosition, Color.White);
        }

        spriteBatch.FillRectangle(Rectangle, OverlayColor);
    }

    public void Center(float width, float height)
        => Position += new Vector2(width / 2 - Width / 2, height / 2 - Height / 2);

    public Tween TweenToCenter(float width, float height,
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null)
        => TweenToPosition(new Vector2(width / 2f - Rectangle.Width / 2f, height / 2f - Rectangle.Height / 2f),
            duration, delay, onEnd, onBegin, repeatCount, repeatDelay, autoReverse, easingFunction);

    public Tween TweenToPosition(Vector2 toValue,
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.Position,
                toValue, duration, delay, onEnd, onBegin,
                repeatCount, repeatDelay, autoReverse, easingFunction);

    public Tween TweenToOffset(Vector2 offset,
        float duration, float delay = 0f,
        Action<Tween>? onEnd = null, Action<Tween>? onBegin = null,
        int repeatCount = 0, float repeatDelay = 0f,
        bool autoReverse = false, Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.Position, Position + offset, duration, delay, onEnd, onBegin,
                repeatCount, repeatDelay, autoReverse, easingFunction);

    public Tween TweenToOverlayAlpha(float alpha, float duration, float delay = 0f,
        Action<Tween>? onEnd = null, Action<Tween>? onBegin = null,
        int repeatCount = 0, float repeatDelay = 0f,
        bool autoReverse = false, Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.OverlayAlpha, alpha, duration, delay, onEnd, onBegin,
                repeatCount, repeatDelay, autoReverse, easingFunction);

    public Tween FadeToColor(float duration, Color? color = null, float delay = 0f,
        Action<Tween>? onEnd = null, Action<Tween>? onBegin = null,
        int repeatCount = 0, float repeatDelay = 0f,
        bool autoReverse = false, Func<float, float>? easingFunction = null)
    {
        OverlayColor = color.HasValue ? new(color.Value, 0) : OverlayColor;
        return TweenToOverlayAlpha((color?.A ?? 255) / 255f, duration, delay, onEnd, onBegin,
                repeatCount, repeatDelay, autoReverse, easingFunction);
    }
}