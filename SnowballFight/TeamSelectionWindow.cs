using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.Ui;
using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.SnowballFight;

class TeamSelectionWindow : Panel
{
    private readonly Label headerLabel;
    private readonly Panel santaWindow;
    private readonly Panel krampusWindow;
    private readonly int screenSize;

    public Action<TeamSelectionWindow, Team>? TeamSelected { get; set; }

    public TeamSelectionWindow(IControlContainer container, int screenSize, Texture2D santaPicture, Texture2D krampusPicture)
        : base(container, 0, 0, 0, 0)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.None;
        this.screenSize = screenSize;

        headerLabel = this.AddLabel(144, 140, "Choose your team!", strokeSize: 3);
        santaWindow = new(this, 159, 215, 300, 300);
        krampusWindow = new(this, 501, 215, 300, 300);

        santaWindow.AddPicture(5, 5, santaPicture, 14);
        santaWindow.AddLabel(1, 227, "Santa", 1);

        krampusWindow.AddPicture(5, 5, krampusPicture, 14);
        krampusWindow.AddLabel(1, 227, "Krampus", 1);
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (Hidden)
            return;

        var (logoWidth, logoHeight) = headerLabel.CalculateSize();
        headerLabel.Position = new(screenSize / 2 - logoWidth / 2, logoHeight * 2);

        santaWindow.Position = headerLabel.Position + new Vector2(15, 75);
        krampusWindow.Position = headerLabel.Position + new Vector2(logoWidth - 315, 75);

        Team? team = null;

        if (HighlightIfHovering(updateParameters, santaWindow))
            team = Team.Santa;

        if (HighlightIfHovering(updateParameters, krampusWindow))
            team = Team.Krampus;

        if (team.HasValue)
            TeamSelected?.Invoke(this, team.Value);

        base.Update(updateParameters);
    }

    private bool HighlightIfHovering(UpdateParameters updateParameters, Panel window)
    {
        var intersects = window.Rectangle.Intersects(updateParameters.LocalMousePosition);
        //TODO: window.Controls.OfType<Label>().ForEach(l => l.ForceHighlight = intersects);
        return intersects && updateParameters.LeftMouseClick;
    }
}
