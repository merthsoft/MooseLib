using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Entities.Spells;

namespace Merthsoft.Moose.Dungeon.Ux;
public class SpellBookPanel : Panel
{
    private readonly List<SpellDef> spellList = new();
    private readonly Panel spellPanel;
    private DungeonPlayer player;
    
    private int selectedSpell;

    public SpellBookPanel(IControlContainer container, float x, float y) : base(container, x, y, 320, 390)
    {
        player = DungeonPlayer.Instance;
        BackgroundDrawingMode = BackgroundDrawingMode.None;

        this.AddLabel("Spells", 5, 5, color: Color.DarkMagenta);
        this.AddLabel("Spells", 0, 0);
        spellPanel = this.AddPanel(0, 60, Width, Height, BackgroundDrawingMode.None);
    }

    private void RebuildSpells()
    {
        spellPanel.ClearControls();
        spellList.Clear();
        spellList.AddRange(player.KnownSpells);
        selectedSpell = player.SelectedSpellIndex;
        var spellIndex = 0;
        
        for (var j = 0; j < 2; j++)
            for (var i = 0; i < 3; i++)
            {
                var index = spellIndex;
                var button = spellPanel.AddButton(i * 145, j * 165, "", (c, u) => {
                    c.Toggled = true;
                    player.SelectedSpellIndex = index < spellList.Count ? index : player.SelectedSpellIndex;
                }, 1);
                button.Toggleable = true;
                button.Toggled = spellIndex == player.SelectedSpellIndex;

                button.WidthOverride = 97;
                button.HeightOverride = 97;
                button.BackgroundDrawingMode = BackgroundDrawingMode.Texture;
                
                if (spellIndex < spellList.Count)
                {
                    var spell = spellList[spellIndex];
                    button.Text = spell.Name;
                    button.LabelOffset = new(2, 0);
                    button.Texture = spell.Icon;
                    button.TextureScale = new(6, 6);
                }
                spellIndex++;
            }
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (player == null)
            player = DungeonPlayer.Instance;
        //if (!spellList.SequenceEqual(player.KnownSpells) || selectedSpell != player.SelectedSpellIndex)
            RebuildSpells();
        base.Update(updateParameters);
    }
}
