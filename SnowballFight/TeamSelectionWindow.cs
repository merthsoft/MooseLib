namespace Merthsoft.Moose.SnowballFight;

class TeamSelectionWindow : StackPanel
{
    private readonly Panel santaWindow;
    private readonly Panel krampusWindow;
    private readonly Label santaLabel;
    private readonly Label krampusLabel;

    public new Action<Team?> Action { get; }

    public TeamSelectionWindow(Action<Team?> action, IControlContainer container, Texture2D santaPicture, Texture2D krampusPicture)
        : base(container, 0, 0, 960, 800)
    {
        Action = action;

        BackgroundDrawingMode = BackgroundDrawingMode.None;

        this.AddLabel(0, 0, "Choose your team!", 5, Color.Yellow, strokeSize: 3);
        var labelSize = MeasureString("Choose your team!", 5) + new Vector2(3, 3);
        Height = labelSize.Y + 500;
        Width = labelSize.X;
        var selectionPanel = this.AddStackPanel(0, 0, labelSize.X, 320, BackgroundDrawingMode.None, padding: (int)labelSize.X - 576);
        santaWindow = selectionPanel.AddPanel(0, 0, 288, 288);
        krampusWindow = selectionPanel.AddPanel(0, 0, 288, 288);

        santaWindow.AddPicture(5, 5, santaPicture, 14);
        santaLabel = santaWindow.AddLabel(1, 232, "Nicholas", 6, hightlightOnHover: true);

        krampusWindow.AddPicture(5, 5, krampusPicture, 14);
        krampusLabel = krampusWindow.AddLabel(1, 257, "Krampus", 4, hightlightOnHover: true);

        var button = this.AddButton(0, 0, "Back", (_, __) => Action.Invoke(null), 1);
        button.BackgroundDrawingMode = BackgroundDrawingMode.Texture;
    }

    public override void Update(UpdateParameters updateParameters)
    {
        base.Update(updateParameters);

        santaLabel.TextColor = Theme.ResolveTextColor(santaWindow.UpdateParameters, true, false, true);
        krampusLabel.TextColor = Theme.ResolveTextColor(krampusWindow.UpdateParameters, true, false, true);

        if (updateParameters.LeftMouseClick)
        {
            if (santaWindow.UpdateParameters.MouseOver)
                Action.Invoke(Team.Santa);
            else if (krampusWindow.UpdateParameters.MouseOver)
                Action.Invoke(Team.Krampus);
        }
    }
}
