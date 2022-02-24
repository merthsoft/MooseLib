namespace Merthsoft.Moose.SnowballFight;

class TeamSelectionWindow : Panel
{
    private readonly Label headerLabel;
    private readonly Panel santaWindow;
    private readonly Panel krampusWindow;
    private readonly Label santaLabel;
    private readonly Label krampusLabel;

    public Action<TeamSelectionWindow, Team>? TeamSelected { get; set; }

    public TeamSelectionWindow(IControlContainer container, Texture2D santaPicture, Texture2D krampusPicture)
        : base(container, 0, 0, 1000, 1000)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.None;

        headerLabel = this.AddLabel(144, 140, "Choose your team!", 0, Color.Yellow, strokeSize: 3);
        santaWindow = this.AddPanel(159, 215, 300, 300);
        krampusWindow = this.AddPanel(501, 215, 300, 300);

        santaWindow.AddPicture(5, 5, santaPicture, 14);
        santaLabel = santaWindow.AddLabel(1, 227, "Santa", 1, hightlightOnHover: true);

        krampusWindow.AddPicture(5, 5, krampusPicture, 14);
        krampusLabel = krampusWindow.AddLabel(1, 227, "Krampus", 1, hightlightOnHover: true);
    }

    public override void PostControlUpdate(UpdateParameters updateParameters)
    {
        santaLabel.TextColor = Theme.ResolveTextColor(santaWindow.UpdateParameters, true, false, true);
        krampusLabel.TextColor = Theme.ResolveTextColor(krampusWindow.UpdateParameters, true, false, true);
    }
}
