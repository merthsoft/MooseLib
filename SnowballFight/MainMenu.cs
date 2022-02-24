namespace Merthsoft.Moose.SnowballFight;

class MainMenu : Panel
{
    public new Action<string> Action { get; }
    public MainMenu(Action<string> action, IControlContainer container)
        : base(container, 0, 0, 0, 0)
    {
        Action = action;
        BackgroundDrawingMode = BackgroundDrawingMode.None;

        var logo = this.AddLabel(0, 0, "Project Triangle Snowfight", 0, Color.Yellow, 3);
        var logoSize = MeasureString("Project Triangle Snowfight") + new Vector2(3, 3);
        var size = MeasureString("New Game");
        var width = size.X + 32;
        var menuPanel = this.AddStackPanel(logoSize.X / 2 - width / 2, logoSize.Y + 3, width, size.Y * 4 + 32, BackgroundDrawingMode.Texture, StackDirection.Horizontal);
        menuPanel.AddActionLabel(0, 0, "New Game", Emit);
        menuPanel.AddActionLabel(0, 0, "Settings", null);
        menuPanel.AddActionLabel(0, 0, "About  ", null);
        menuPanel.AddActionLabel(0, 0, "Exit", Emit);

        Size = new(logoSize.X, menuPanel.Size.Y + logoSize.Y + 3);
    }

    private void Emit(Control c, UpdateParameters _)
        => Action.Invoke((c as Label)?.Text ?? "");
}
