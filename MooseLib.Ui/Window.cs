using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui;

public class Window : IControlContainer
{
    private Rectangle rectangle;

    protected List<Control> controlsToAdd = new();
    protected List<Control> controls = new();
    public Control[] Controls => controls.ToArray();

    public Theme Theme { get; set; }

    public bool ShouldClose { get; protected set; }

    public bool IsHidden { get; set; } = false;

    public BackgroundDrawingMode BackgroundDrawingMode { get; set; } = BackgroundDrawingMode.Texture;

    public Action<Window>? OnClose { get; set; }

    public WindowManager? Manager { get; set; }

    private Window? contextMenu;
    public Window? ContextMenu { 
        get => contextMenu;
        set
        {
            contextMenu = value;
        }
    }

    public Rectangle Rectangle
    {
        get => rectangle;
        set
        {
            var oldRect = rectangle;
            rectangle = value;
            RectangleChanged?.Invoke(this, new(oldRect, value));
        }
    }

    public Action<Window, ValueChangedParameters<Rectangle>>? RectangleChanged;

    public Vector2 Position
    {
        get => new(Rectangle.X, Rectangle.Y);
        set => Rectangle = Rectangle with { X = (int)value.X, Y = (int)value.Y };
    }

    public Vector2 Size
    {
        get => new(Rectangle.Width, Rectangle.Height);
        set => Rectangle = Rectangle with { Height = (int)value.X, Width = (int)value.Y };
    }

    public int Width => Rectangle.Width;
    public int Height => Rectangle.Height;
    public int X => Rectangle.X;
    public int Y => Rectangle.Y;

    public Window(Theme theme, int x, int y, int w = 0, int h = 0)
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

    public virtual void Update(UpdateParameters updateParameters)
    {
        PreControlUpdate(updateParameters);
        
        foreach (var c in Controls)
        {
            UpdateParameters controlUpdateParameters
                = new(updateParameters.GameTime, updateParameters.LocalMousePosition - c.Position, updateParameters.RawMouseState, updateParameters.RawKeyState);
            if (c.Rectangle.Contains(updateParameters.LocalMousePosition) && !IsHidden && !c.IsHidden && updateParameters.MouseOver && ContextMenu == null)
            {
                controlUpdateParameters.MouseOver = true;
                controlUpdateParameters.LeftMouseClick = updateParameters.LeftMouseClick;
                controlUpdateParameters.RightMouseClick = updateParameters.RightMouseClick;
                controlUpdateParameters.LeftMouseDown = updateParameters.LeftMouseDown;
                controlUpdateParameters.RightMouseDown = updateParameters.RightMouseDown;
            }
            c.Update(controlUpdateParameters);
            c.UpdateParameters = controlUpdateParameters;
        }
        controls.AddRange(controlsToAdd);
        controlsToAdd.Clear();
        PostControlUpdate(updateParameters);
    }

    public virtual void PreControlUpdate(UpdateParameters updateParameters) { }
    public virtual void PostControlUpdate(UpdateParameters updateParameters) { }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden)
            return;

        var pos = Position + Theme.DrawWindow(spriteBatch, Rectangle, BackgroundDrawingMode);

        foreach (var c in Controls)
            if (!c.IsHidden)
                c.Draw(spriteBatch, pos);
    }

    public void Center(int width, int height)
        => Position += new Vector2(width / 2 - Width / 2, height / 2 - Height / 2);

    public void Center(Vector2 size)
        => Position += new Vector2(size.X.Ceiling() / 2 - Width / 2, size.Y.Ceiling() / 2 - Height / 2);

    public void Close()
    {
        ShouldClose = true;
        IsHidden = true;
    }

    public void Hide()
        => IsHidden = true;

    public void Show()
        => IsHidden = false;
}
