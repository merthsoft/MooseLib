using Merthsoft.Moose.Dungeon.Entities;

namespace Merthsoft.Moose.Dungeon.Ux;
public class StatsPanel : Panel
{
    DungeonPlayer? player;

    public StatsPanel(IControlContainer container, float x, float y) : base(container, x, y, 320, 320)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.None;
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (player == null || player.StatsUpdated || true)
        {
            player = DungeonPlayer.Instance;
            player.StatsUpdated = false;
            ClearControls();
            var height = Theme.MeasureString("X", 2).Y;

            var label = this.AddLabel(0, 0, player.Name);
            label.HighlightOnHover = false;
            var statsPanel = this.AddGrowPanel(0, 75, BackgroundDrawingMode.None);
            var hpPanel = statsPanel.AddStackPanel(0, 0, 320, height, BackgroundDrawingMode.None);
            hpPanel.AddLabel(0, 0, "HP:1", 2, Color.IndianRed);
            hpPanel.AddLabel(0, 0, $"+{player.Armor}", 2, Color.White);
            hpPanel.AddLabel(0, 0, $"+{player.MagicArmor}", 2, Color.MediumPurple);
            hpPanel.AddLabel(0, 0, $"={1 + player.Armor + player.MagicArmor}", 2);
            var mpPanel = statsPanel.AddStackPanel(0, 0, 320, height, BackgroundDrawingMode.None);
            mpPanel.AddLabel(0, 0, $"MP:{player.Mana} ", 2, Color.CornflowerBlue);
            mpPanel.AddLabel(0, 0, $"GP:{player.Gold}", 2, Color.Gold);

            statsPanel.AddLabel(0, 0, $"Floor: {(player.DungeonLevel == 0 ? "Town" : player.DungeonLevel.ToString())}", 2);

            base.Update(updateParameters);
        }
    }
}
