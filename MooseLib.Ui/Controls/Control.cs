using Merthsoft.Moose.MooseEngine.Interface;
using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public abstract class Control : ITweenOwner
{
    public IControlContainer Container { get; }
 
    public int FontIndex { get; set; }
    public SpriteFont Font => Theme.Fonts[FontIndex];

    public object? Tag { get; set; }
    public T? GetTag<T>() => (T)Tag;

    public bool Hidden { get; set; }
    public bool Enabled { get; set; } = true;
    public bool Remove { get; set; } = false;
    public bool Toggleable { get; set; }
    public bool Toggled { get; set; }

    public BackgroundDrawingMode BackgroundDrawingMode { get; set; } = BackgroundDrawingMode.Basic;

    public Theme Theme => Container.Theme;

    public Vector2 Position { get; set; }

    public float X
    {
        get => Position.X;
        set => Position = Position with { X = value };
    }

    public float Y
    {
        get => Position.Y;
        set => Position = Position with { Y = value };
    }

    public UpdateParameters UpdateParameters { get; set; } = new();

    public Color? TextColor { get; set; }
    public Color? BackgroundColor { get; set; }
    public Color? BorderColor { get; set; }

    protected virtual Color ResolvedBackgroundColor => BackgroundColor ?? Theme.ResolveBackgroundColor(UpdateParameters, Enabled);
    protected virtual Color ResolvedBorderColor => BorderColor ?? Theme.ControlBorderColor;
    protected virtual Color ResolvedTextColor => TextColor ?? Theme.ResolveTextColor(UpdateParameters, Enabled, Toggled);
    protected virtual Color ResolvedPointerColor => Theme.ControlPointerColor;

    public RectangleF Rectangle
    {
        get
        {
            var CalculatedSize = CalculateSize();
            return new(Position.X, Position.Y, CalculatedSize.X, CalculatedSize.Y);
        }
    }

    public Action<Control, UpdateParameters>? Action { get; set; }
    public List<Tween> ActiveTweens { get; } = new();

    public Control(IControlContainer container, float x, float y)
    {
        Container = container;
        Position = new(x, y);
    }

    public Control Hide()
    {
        Hidden = true;
        return this;
    }

    public Control Show()
    {
        Hidden = false;
        return this;
    }

    public Vector2 MeasureString(string s, int? fontIndex = null)
        => Theme.MeasureString(s, fontIndex ?? FontIndex);

    public string TruncateString(string s, int width, string truncationString = "...", int? fontIndex = null)
        => Theme.TruncateString(s, width, truncationString, fontIndex ?? FontIndex);

    public abstract Vector2 CalculateSize();

    public virtual void Update(UpdateParameters updateParameters)
    {
        if (updateParameters.MouseOver && (updateParameters.LeftMouseClick || updateParameters.RightMouseClick))
        {
            if (Toggleable)
                Toggled = !Toggled;
            Action?.Invoke(this, updateParameters);
        }
    }

    public virtual void PostUpdate()
    {
        if (Remove)
            this.ClearTweens();
        else
            this.ClearCompletedTweens();
    }

    public abstract void Draw(SpriteBatch spriteBatch, Vector2 parentOffset);

    public void Center(float width, float height)
        => Position += new Vector2(width / 2f - Rectangle.Width / 2f, height / 2f - Rectangle.Height / 2f);

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
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null)
        => this.AddTween(o => o.Position,
                Position + offset, duration, delay, onEnd, onBegin, 
                repeatCount, repeatDelay, autoReverse, easingFunction);
}
