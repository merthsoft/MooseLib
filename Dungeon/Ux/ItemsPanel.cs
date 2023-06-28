using Merthsoft.Moose.Dungeon.Entities;

namespace Merthsoft.Moose.Dungeon.Ux;
public class ItemsPanel : Panel
{
    DungeonPlayer? player;
    int itemOffset = 0;

    public ItemsPanel(IControlContainer container, float x, float y) : base(container, x, y, 320, 225)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.None;
    }

    public override void Update(UpdateParameters updateParameters)
    {
        if (player == null || player.ItemsUpdated || true)
        {
            player = DungeonPlayer.Instance;
            player.ItemsUpdated = false;
            ClearControls();

            this.AddLabel("Items", 5, 5, color: Color.DarkMagenta);
            this.AddLabel("Items", 0, 0);

            var itemPanel = this.AddPanel(0, 60, 320, 100, BackgroundDrawingMode.None);
            var itemIndex = 0;
            var itemList = player.Items;
            for (var i = 0; i < 3; i++)
            {
                var index = itemIndex;
                var button = itemPanel.AddButton(i * 145, 0,"", fontIndex: 1);

                button.WidthOverride = 97;
                button.HeightOverride = 97;
                button.BackgroundDrawingMode = BackgroundDrawingMode.Texture;

                if (itemIndex < itemList.Count)
                {
                    var item = itemList[itemIndex];
                    button.Text = item.ItemDef.Name.Replace(' ', '\n');
                    button.LabelOffset = new(2, 0);
                    button.Texture = WiggleWizzardGame.Instance.ItemTiles;
                    button.SourceRectangle = button.Texture.GetSourceRectangle(item.DrawIndex, 16, 16);
                    button.TextureScale = new(6, 6);
                    button.Action = (c, u) =>
                    {
                        if (player.CanMove && !player.Blinking && !player.Targeting)
                            item.Use();
                    };
                }
                itemIndex++;
            }

        }
        base.Update(updateParameters);
    }
    
    public void UseItem(int value)
        => ((Controls[2] as Panel)?.Controls[value] as Button)?.Action?.Invoke(this, null!);
}