using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public abstract class Control
{
    public Theme Theme { get; set; }

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

    public int FontIndex { get; set; }
    public SpriteFont Font => Theme.Fonts[FontIndex];

    public bool Hidden { get; set; }
    public bool Enabled { get; set; } = true;
    public bool Remove { get; set; } = false;

    public UpdateParameters UpdateParameters { get; set; } = new();

    public RectangleF Rectangle
    {
        get
        {
            var CalculatedSize = CalculateSize();
            return new(Position.X, Position.Y, CalculatedSize.X, CalculatedSize.Y);
        }
    }

    public Action<Control, UpdateParameters>? Action { get; set; }

    public Control(Theme theme, float x, float y)
    {
        Theme = theme;
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
            Action?.Invoke(this, updateParameters);
    }

    public abstract void Draw(SpriteBatch spriteBatch, Vector2 parentOffset);

    public void Center(float width, float height)
        => Position += new Vector2(width / 2 - Rectangle.Width / 2, height / 2 - Rectangle.Height / 2);
}
