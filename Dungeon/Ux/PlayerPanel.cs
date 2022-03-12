using Merthsoft.Moose.Dungeon.Entities;

namespace Merthsoft.Moose.Dungeon.Ux;
public class PlayerPanel : Panel
{
    DungeonPlayer player;

    public PlayerPanel(IControlContainer container, float x, float y) : base(container, x, y, 320, 320)
    {
        player = DungeonPlayer.Instance;
        BackgroundDrawingMode = BackgroundDrawingMode.None;
    }

    public override void Update(UpdateParameters updateParameters)
    {
        ClearControls();
        var label = this.AddLabel(0, 0, "Player");
        label.HighlightOnHover = false;
        var statsPanel = this.AddGrowPanel(0, 75, BackgroundDrawingMode.None);
        statsPanel.AddLabel(0, 0, "Name:", 2);
        statsPanel.AddActionLabel(0, 0, player.Name, (_, __) => { }, 2);
        
        statsPanel.AddRectangle(0, 0, 5, 5, Color.Black);
        statsPanel.AddLabel(0, 0, $"MP:{player.Mana}", 2);
        statsPanel.AddLabel(0, 0, $"GP:{player.Gold}", 2);

        base.Update(updateParameters);
    }
}
