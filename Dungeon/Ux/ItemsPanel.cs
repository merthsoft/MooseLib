using Merthsoft.Moose.Dungeon.Entities;

namespace Merthsoft.Moose.Dungeon.Ux;
internal class ItemsPanel : Panel
{
    DungeonPlayer? player;
    int itemOffset = 0;

    public ItemsPanel(IControlContainer container, float x, float y) : base(container, x, y, 320, 288)
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

            var label = this.AddLabel(0, 0, "Items");
            label.HighlightOnHover = false;

            var panel = this.AddPanel(0, 70, 320, 288, BackgroundDrawingMode.None);
            panel.AddTextureButton(0, 0, DungeonGame.Instance.PreviousButtonTexture, (_, __) => itemOffset--);
            var j = 0;

            if (itemOffset < 0)
                itemOffset = 0;

            if (itemOffset >= player.Items.Count)
                itemOffset = player.Items.Count - 1;

            var items = player.Items.Skip(itemOffset).Take(4).GetEnumerator();
            for (j = 0; j < 4; j++)
            {
                panel.AddRectangle(j * 65 + 14, 0, 64, 64, DungeonGame.Instance.DefaultBackgroundColor, DungeonGame.Instance.DefaultBackgroundColor);
                if (items.MoveNext())
                {
                    var item = items.Current.DrawIndex;
                    panel.AddPicture(j * 65+ 14, 0, DungeonGame.Instance.ItemTiles,
                         DungeonGame.Instance.ItemTiles.GetSourceRectangle(item, 16, 16), new(4, 4));
                }

            }
            panel.AddTextureButton(j*65+14, 0, DungeonGame.Instance.NextButtonTexture, (_, __) => itemOffset++);

            base.Update(updateParameters);
        }
    }
}
