using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Panel : Control, IControlContainer
{
    public float Width { get; set; }
    public float Height { get; set; }

    public Vector2 Size
    {
        get => new(Width, Height);
        set
        {
            Width = value.X;
            Height = value.Y;
        }
    }

    //protected List<Control> controlsToAdd = new();
    protected List<Control> controls = new();
    public Control[] Controls => controls.ToArray();

    public BackgroundDrawingMode BackgroundDrawingMode { get; set; } = BackgroundDrawingMode.Basic;
    protected Control? FocusedControl { get; set; }

    public Panel(Theme theme, float x, float y, float w, float h)
        : base(theme, x, y)
    {
        Width = w;
        Height = h;
    }

    public override Vector2 CalculateSize()
        => new(Width, Height);

    public virtual IControlContainer AddControl(Control control)
    {
        //controlsToAdd.Add(control);
        controls.Add(control);
        return this;
    }

    public virtual IControlContainer ClearControls()
    {
        controls.Clear();
        //controlsToAdd.Clear();
        return this;
    }

    public virtual IControlContainer RemoveControl(Control control)
    {
        controls.Remove(control);
        //controlsToAdd.Remove(control);
        return this;
    }

    public virtual IControlContainer RemoveControlAt(int index)
    {
        controls.RemoveAt(index);
        return this;
    }

    public override void Update(UpdateParameters updateParameters)
    {
        PreControlUpdate(updateParameters);

        var position = Position + updateParameters.ParentOffset;
        position += Theme.GetDrawOffset(BackgroundDrawingMode);

        if (!updateParameters.LeftMouseDown && !updateParameters.RightMouseDown)
            FocusedControl = null;

        foreach (var c in Controls.Reverse())
            updateControl(c);

        controls.RemoveAll(c => c.Remove);
        PostControlUpdate(updateParameters);

        void updateControl(Control c)
        {
            var controlUpdateParameters = new UpdateParameters(
               updateParameters.GameTime, position, updateParameters.LocalMousePosition - c.Position,
               updateParameters.RawMouseState, updateParameters.RawKeyState,
               FocusedControl);
            if (c.Rectangle.Contains(updateParameters.LocalMousePosition)
                && !Hidden && !c.Hidden
                && updateParameters.MouseOver
                && (FocusedControl == null || FocusedControl == c))
            {
                if (FocusedControl == null)
                    updateParameters.FocusedControl = controlUpdateParameters.FocusedControl = FocusedControl = c;
                controlUpdateParameters.MouseOver = true;
                controlUpdateParameters.LeftMouseClick = updateParameters.LeftMouseClick;
                controlUpdateParameters.RightMouseClick = updateParameters.RightMouseClick;
                controlUpdateParameters.LeftMouseDown = updateParameters.LeftMouseDown;
                controlUpdateParameters.RightMouseDown = updateParameters.RightMouseDown;
            }
            c.Update(controlUpdateParameters);
            c.UpdateParameters = controlUpdateParameters;

            if (controlUpdateParameters.Prompt != null)
            {
                updateParameters.Prompt = controlUpdateParameters.Prompt;
                updateParameters.CenterPrompt = controlUpdateParameters.CenterPrompt;
            }
        }
    }

    public virtual void PreControlUpdate(UpdateParameters updateParameters) { }
    public virtual void PostControlUpdate(UpdateParameters updateParameters) { }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
    {
        if (Hidden)
            return;

        var position = Position + parentOffset;
        position += Theme.DrawWindow(spriteBatch, Rectangle.Move(parentOffset), BackgroundDrawingMode);

        foreach (var c in Controls)
            if (!c.Hidden)
                c.Draw(spriteBatch, position);
    }
}
