using Merthsoft.Moose.Dungeon.Entities;

namespace Merthsoft.Moose.Dungeon.Ux;
internal class ItemsPanel : GrowPanel
{
    DungeonPlayer? player;
    int itemOffset = 0;

    public ItemsPanel(IControlContainer container, float x, float y) : base(container, x, y)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.None;
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (player == null || player.ItemsUpdated)
        {
            player = DungeonPlayer.Instance;
            player.ItemsUpdated = false;
            ClearControls();

            var label = this.AddLabel("Items", 0, 0);
            label.HighlightOnHover = false;

            var itemPanel = this.AddPanel(0, 70, 320, 320, BackgroundDrawingMode.None);
            var itemIndex = 0;
            var itemList = player.Items;
            for (var i = 0; i < 3; i++)
            {
                var index = itemIndex;
                var button = itemPanel.AddButton(i * 97, 0, "", (c, u) => {
                    c.Toggled = false;
                    player.SelectedSpellIndex = index < itemList.Count ? index : player.SelectedSpellIndex;
                }, 1);
                button.Toggleable = true;

                button.WidthOverride = 75;
                button.HeightOverride = 114;
                button.BackgroundDrawingMode = BackgroundDrawingMode.Texture;

                if (itemIndex < itemList.Count)
                {
                    var item = itemList[itemIndex];
                    button.Text = item.ItemDef.Name.Replace(' ', '\n');
                    button.LabelOffset = new(2, 0);
                    button.Texture = DungeonGame.Instance.ItemTiles;
                    button.SourceRectangle = button.Texture.GetSourceRectangle(item.DrawIndex, 16, 16);
                    button.TextureScale = new(4, 4);
                }
                itemIndex++;
            }

        }
        base.Update(updateParameters);
    }
}