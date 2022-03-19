namespace Merthsoft.Moose.MooseEngine.Ui.Controls.Prompts;

public class TextPrompt : Panel
{
    private readonly TextBox textBox;
    private readonly Action<string> okClicked;

    public TextPrompt(string label, string defaultText, Action<string> okClicked, IControlContainer container, float x, float y)
        : base(container, x, y, 224, 86)
    {
        this.okClicked = okClicked;
        this.AddLabel(label, 0, 0, 1);
        textBox = this.AddTextBox(0, 25, 214, defaultText, 1);
        this.AddButton(187, 50, "Ok", HandleOkButton, 1);
        this.AddStackPanel(0, 50, 186, 50, BackgroundDrawingMode.None, StackDirection.Horizontal, 3)
            .AddControl(new Button("Clear", this, 0, 0) { FontIndex = 1, Action = (_, __) => textBox.Text = "" })
            .AddControl(new Button("Reset", this, 0, 0) { FontIndex = 1, Action = (_, __) => textBox.Text = defaultText });

        BackgroundDrawingMode = BackgroundDrawingMode.Texture;
    }

    private void HandleOkButton(Control control, UpdateParameters updateParameters)
        => Ok();

    private void Ok()
    {
        Remove = true;
        okClicked(textBox.Text);
    }

    public override void Update(UpdateParameters updateParameters)
    {
        base.Update(updateParameters);

        if (updateParameters.RawKeyState.IsKeyDown(Keys.Enter))
            Ok();
    }
}
