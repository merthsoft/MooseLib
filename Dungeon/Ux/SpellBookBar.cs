using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Spells;

namespace Merthsoft.Moose.Dungeon.Ux;
public class SpellBookBar : Panel
{
    private readonly List<SpellDef> spellList = new();
    private readonly Panel spellPanel;
    private readonly DungeonPlayer player;

    public SpellBookBar(IControlContainer container, float x, float y) : base(container, x, y, 320, 600)
    {
        player = DungeonPlayer.Instance;
        BackgroundDrawingMode = BackgroundDrawingMode.None;

        this.AddActionLabel(0, -75, "Spells", (_, __) => { });

        spellPanel = this.AddPanel(0, 75, 320, Height, BackgroundDrawingMode.None);
        RebuildSpells();
    }

    private void RebuildSpells()
    {
        spellPanel.ClearControls();
        spellList.Clear();
        spellList.AddRange(player.KnownSpells);
        var spellIndex = 0;
        
        for (var j = 0; j < 3; j++)
            for (var i = 0; i < 2; i++)
            {
                var index = spellIndex;
                var button = spellPanel.AddButton(i * 150, j * 165, "", (c, u) => {
                    c.Toggled = false;
                    player.SelectedSpell = index < spellList.Count ? index : player.SelectedSpell;
                }, 1);
                button.Toggleable = true;
                button.Toggled = spellIndex == player.SelectedSpell;

                button.WidthOverride = 128;
                button.HeightOverride = 158;
                button.BackgroundDrawingMode = BackgroundDrawingMode.Texture;
                
                if (spellIndex < spellList.Count)
                {
                    var spell = spellList[spellIndex];
                    button.Text = spell.Name;
                    button.Texture = spell.Icon;
                    button.TextureScale = new(6, 6);
                }
                spellIndex++;
            }
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (!spellList.SequenceEqual(player.KnownSpells))
            RebuildSpells();
        base.Update(updateParameters);
    }
}
