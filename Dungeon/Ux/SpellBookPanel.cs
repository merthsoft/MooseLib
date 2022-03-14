using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Spells;

namespace Merthsoft.Moose.Dungeon.Ux;
public class SpellBookPanel : Panel
{
    private readonly List<SpellDef> spellList = new();
    private readonly Panel spellPanel;
    private readonly DungeonPlayer player;
    
    private int selectedSpell;

    public SpellBookPanel(IControlContainer container, float x, float y) : base(container, x, y, 320, 285)
    {
        player = DungeonPlayer.Instance;
        BackgroundDrawingMode = BackgroundDrawingMode.None;

        var spellsLabel = this.AddLabel(0, 0, "Spells");
        spellsLabel.HighlightOnHover = false;
        spellPanel = this.AddPanel(0, 60, 320, Height, BackgroundDrawingMode.None);
    }

    private void RebuildSpells()
    {
        spellPanel.ClearControls();
        spellList.Clear();
        spellList.AddRange(player.KnownSpells);
        selectedSpell = player.SelectedSpell;
        var spellIndex = 0;
        
        for (var j = 0; j < 2; j++)
            for (var i = 0; i < 3; i++)
            {
                var index = spellIndex;
                var button = spellPanel.AddButton(i * 97, j * 110, "", (c, u) => {
                    c.Toggled = false;
                    player.SelectedSpell = index < spellList.Count ? index : player.SelectedSpell;
                }, 1);
                button.Toggleable = true;
                button.Toggled = spellIndex == player.SelectedSpell;

                button.WidthOverride = 75;
                button.HeightOverride = 75;
                button.BackgroundDrawingMode = BackgroundDrawingMode.Texture;
                
                if (spellIndex < spellList.Count)
                {
                    var spell = spellList[spellIndex];
                    button.Text = spell.Name;
                    button.LabelOffset = new(2, 0);
                    button.Texture = spell.Icon;
                    button.TextureScale = new(4, 4);
                }
                spellIndex++;
            }
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (!spellList.SequenceEqual(player.KnownSpells) || selectedSpell != player.SelectedSpell)
            RebuildSpells();
        base.Update(updateParameters);
    }
}
