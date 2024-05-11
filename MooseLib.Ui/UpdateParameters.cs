namespace Merthsoft.Moose.MooseEngine.Ui;

public record UpdateParameters(GameTime GameTime, Vector2 ParentOffset, Vector2 LocalMousePosition, MouseState RawMouseState, KeyboardState RawKeyState, Control? FocusedControl)
{
    public bool MouseOver { get; set; }

    public bool LeftMouseClick { get; set; }

    public bool RightMouseClick { get; set; }

    public bool LeftMouseDown { get; set; }

    public bool RightMouseDown { get; set; }

    public Control? Prompt { get; set; }
    public bool CenterPrompt { get; set; } = false;

    public Control? FocusedControl { get; set; } = FocusedControl;

    public UpdateParameters() : this(new(), default, default, default, default, null) { }
}
