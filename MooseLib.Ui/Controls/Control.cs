using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public abstract class Control
{
    public Theme Theme { get; set; }

    public Vector2 Position { get; set; }

    public int X
    {
        get => (int)Position.X;
        set => Position = Position with { X = value };
    }

    public int Y
    {
        get => (int)Position.Y;
        set => Position = Position with { Y = value };
    }

    public int FontIndex { get; set; }
    public SpriteFont Font => Theme.Fonts[FontIndex];

    public bool IsHidden { get; set; }
    public bool Enabled { get; set; } = true;

    public UpdateParameters UpdateParameters { get; set; } = new(new(), Vector2.Zero, default, default);

    protected Rectangle CalculatedRectangle { get; set; }
    public Rectangle Rectangle
    {
        get
        {
            var size = CalculateSize();
            return CalculatedRectangle = new((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);
        }
    }

    public Action<Control, UpdateParameters>? Action { get; set; }

    public Control(Theme theme, int x, int y)
    {
        Theme = theme;
        Position = new(x, y);
    }

    public Control Hide()
    {
        IsHidden = true;
        return this;
    }

    public Control Show()
    {
        IsHidden = false;
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
            Action?.Invoke(this, updateParameters);
    }

    public abstract void Draw(SpriteBatch spriteBatch, Vector2 parentOffset);
}
