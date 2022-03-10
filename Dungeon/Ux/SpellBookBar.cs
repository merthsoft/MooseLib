using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Spells;

namespace Merthsoft.Moose.Dungeon.Ux;
public class SpellBookBar : Panel
{
    private readonly List<SpellDef> spellList = new();
    private readonly Panel spellPanel;
    private readonly DungeonPlayer player;

    public SpellBookBar(IControlContainer container, float x, float y) : base(container, x, y, 320, 475)
    {
        player = DungeonPlayer.Instance;
        BackgroundDrawingMode = BackgroundDrawingMode.None;

        this.AddActionLabel(0, -75, "Spells", (_, __) => { });

        spellPanel = this.AddPanel(0, 75, 320, 405, BackgroundDrawingMode.None);
        RebuildSpells();
    }

    private void RebuildSpells()
    {
        spellPanel.ClearControls();
        spellList.Clear();
        spellList.AddRange(player.KnownSpells);
        var spellIndex = 0;
        for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            {
                var panel = spellPanel.AddPanel(i * 150, j * 165, 128, 128, BackgroundDrawingMode.Texture);
                if (spellIndex < spellList.Count)
                {
                    var spell = spellList[spellIndex];
                    panel.AddPicture(0, 0, spell.Icon, 6f);
                    spellPanel.AddLabel(i * 150 + 5, j * 165 + 110, spell.Name, 1);
                }
                spellIndex++;
            }
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (!spellList.SequenceEqual(player.KnownSpells))
            RebuildSpells();
        base.Update(updateParameters);

        for (var index = 0; index < spellPanel.Controls.Length; index += 2)
        {
            var panel = spellPanel.GetControl<Panel>(index);
            var label = spellPanel.Controls[index + 1];
            if (panel.UpdateParameters.MouseOver || label.UpdateParameters.MouseOver)
            {
                label.UpdateParameters.MouseOver = true;
                panel.ColorShift = Color.DeepPink;
            }
            else
                panel.ColorShift = index == player.SelectedSpell ? panel.ColorShift = Color.Gold : Color.White;

            if (panel.UpdateParameters.LeftMouseClick || label.UpdateParameters.LeftMouseClick)
                player.SelectedSpell = index;
        }
    }
}
