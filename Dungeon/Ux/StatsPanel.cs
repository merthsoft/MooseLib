using Merthsoft.Moose.Dungeon.Entities;

namespace Merthsoft.Moose.Dungeon.Ux;
public class StatsPanel : Panel
{
    DungeonPlayer? player;

    public StatsPanel(IControlContainer container, float x, float y) : base(container, x, y, 320, 150)
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

            this.AddLabel(player.Name, 5, 5, color: Color.DarkMagenta);
            this.AddLabel(player.Name, 0, 0);

            var statsPanel = this.AddGrowPanel(0, 60, BackgroundDrawingMode.None);
            var hpPanel = statsPanel.AddStackPanel(0, 0, 320, height, BackgroundDrawingMode.None);
            hpPanel.AddLabel("HP:1", 0, 0, 2, Color.IndianRed);
            hpPanel.AddLabel($"+{player.Armor}", 0, 0, 2, Color.White);
            hpPanel.AddLabel($"+{player.MagicArmor}", 0, 0, 2, Color.MediumPurple);
            hpPanel.AddLabel($"={1 + player.Armor + player.MagicArmor}", 0, 0, 2);
            var mpPanel = statsPanel.AddStackPanel(0, 0, 320, height, BackgroundDrawingMode.None);
            mpPanel.AddLabel($"MP:{player.Mana} ", 0, 0, 2, Color.CornflowerBlue);
            mpPanel.AddLabel($"GP:{player.Gold}", 0, 0, 2, Color.Gold);

            statsPanel.AddLabel($"Floor: {(player.DungeonLevel == 0 ? "Town" : player.DungeonLevel.ToString())}", 0, 0, 2);

            base.Update(updateParameters);
        }
    }
}
