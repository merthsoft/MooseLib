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

    protected List<Control> controls = new();
    public Control[] Controls => controls.ToArray();

    protected Control? FocusedControl { get; set; }

    public Panel(IControlContainer container, float x, float y, float w, float h)
        : base(container, x, y)
    {
        Width = w;
        Height = h;
    }

    public override Vector2 CalculateSize()
        => new(Width, Height);

    public virtual IControlContainer AddControl(Control control)
    {
        controls.Add(control);
        return this;
    }

    public virtual IControlContainer ClearControls()
    {
        controls.Clear();
        return this;
    }

    public override void Update(UpdateParameters updateParameters)
    {
        var position = Position + updateParameters.ParentOffset;
        var themeOffset = Theme.GetDrawOffset(BackgroundDrawingMode);
        position += themeOffset;

        if (!updateParameters.LeftMouseDown && !updateParameters.RightMouseDown)
            FocusedControl = null;

        foreach (var c in Controls.Reverse())
            updateControl(c);

        controls.RemoveAll(c => c.Remove);

        void updateControl(Control c)
        {
            if (c.Remove)
                return;

            var controlUpdateParameters = new UpdateParameters(
               updateParameters.GameTime, position, updateParameters.LocalMousePosition - c.Position,
               updateParameters.RawMouseState, updateParameters.RawKeyState,
               FocusedControl);
            if (c.Rectangle.Contains(updateParameters.LocalMousePosition - themeOffset)
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

            if (c is Panel p && p.FocusedControl != null)
                FocusedControl = c;

            c.PostUpdate();
        }
    }

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
