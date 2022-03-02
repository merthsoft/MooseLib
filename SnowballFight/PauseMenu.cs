namespace Merthsoft.Moose.SnowballFight;

class PauseMenu : Panel
{
    public new Action<string> Action { get; }

    public PauseMenu(Action<string> action, IControlContainer container)
        : base(container, 0, 0, 0, 0)
    {
        Action = action;
        BackgroundDrawingMode = BackgroundDrawingMode.None;

        var logo = this.AddLabel(0, 0, "Paused", 5, Color.Yellow, 3);
        var logoSize = MeasureString("Paused", 5) + new Vector2(3, 3);
        var width = 320;
        var menuPanel = this.AddStackPanel(logoSize.X / 2 - width / 2, logoSize.Y + 3, width, 135, BackgroundDrawingMode.Texture, StackDirection.Horizontal);
        menuPanel.Padding = -3;
        menuPanel.AddActionLabel(0, 0, "Resume", Emit, 1);
        menuPanel.AddActionLabel(0, 0, "Settings", null, 1);
        menuPanel.AddActionLabel(0, 0, "Quit", Emit, 1);

        Size = container.Theme.CalculateNewSize(new(logoSize.X, menuPanel.Size.Y + logoSize.Y + 16));
    }

    private void Emit(Control c, UpdateParameters _)
        => Action.Invoke((c as Label)?.Text ?? "");
}
